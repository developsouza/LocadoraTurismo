using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentalTourismSystem.Data;
using RentalTourismSystem.Models;

namespace RentalTourismSystem.Controllers
{
    [Authorize] // Todo o controller requer autenticação
    public class ClientesController : Controller
    {
        private readonly RentalTourismContext _context;
        private readonly ILogger<ClientesController> _logger;

        public ClientesController(RentalTourismContext context, ILogger<ClientesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Clientes - Todos os funcionários podem ver
        public async Task<IActionResult> Index(string? busca)
        {
            try
            {
                _logger.LogInformation("Lista de clientes acessada por usuário {User}", User.Identity?.Name);

                var clientes = _context.Clientes.AsQueryable();

                if (!string.IsNullOrEmpty(busca))
                {
                    clientes = clientes.Where(c => c.Nome.Contains(busca) ||
                                                 c.Cpf.Contains(busca) ||
                                                 (c.Email != null && c.Email.Contains(busca)));

                    _logger.LogInformation("Busca por clientes realizada: '{Busca}' por usuário {User}", busca, User.Identity?.Name);
                }

                ViewBag.Busca = busca;
                var listaClientes = await clientes.OrderBy(c => c.Nome).ToListAsync();

                return View(listaClientes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar lista de clientes para usuário {User}", User.Identity?.Name);
                TempData["Erro"] = "Erro ao carregar lista de clientes. Tente novamente.";
                ViewBag.Busca = busca;
                return View(new List<Cliente>());
            }
        }

        // GET: Clientes/Details/5 - Todos podem ver detalhes
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Tentativa de acesso a detalhes de cliente com ID nulo por {User}", User.Identity?.Name);
                return NotFound();
            }

            try
            {
                var cliente = await _context.Clientes
                    .Include(c => c.Locacoes)
                        .ThenInclude(l => l.Veiculo)
                    .Include(c => c.ReservasViagens)
                        .ThenInclude(r => r.PacoteViagem)
                    .Include(c => c.ReservasViagens)
                        .ThenInclude(r => r.StatusReservaViagem)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (cliente == null)
                {
                    _logger.LogWarning("Cliente com ID {ClienteId} não encontrado. Acessado por {User}", id, User.Identity?.Name);
                    return NotFound();
                }

                _logger.LogInformation("Detalhes do cliente {ClienteId} acessados por {User}", id, User.Identity?.Name);
                return View(cliente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar detalhes do cliente {ClienteId} para usuário {User}", id, User.Identity?.Name);
                TempData["Erro"] = "Erro ao carregar dados do cliente.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Clientes/Create - Todos os funcionários podem criar
        [Authorize(Roles = "Admin,Manager,Employee")]
        public IActionResult Create()
        {
            _logger.LogInformation("Formulário de criação de cliente acessado por {User}", User.Identity?.Name);
            return View(new Cliente { DataNascimento = DateTime.Now.AddYears(-25) });
        }

        // POST: Clientes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager,Employee")]
        public async Task<IActionResult> Create([Bind("Nome,Cpf,Telefone,Email,Endereco,DataNascimento,NumeroHabilitacao,ValidadeCNH")] Cliente cliente)
        {
            try
            {
                // Limpar dados antes da validação
                if (!string.IsNullOrEmpty(cliente.Cpf))
                    cliente.Cpf = cliente.Cpf.Replace(".", "").Replace("-", "");

                if (!string.IsNullOrEmpty(cliente.Telefone))
                    cliente.Telefone = cliente.Telefone.Replace("(", "").Replace(")", "").Replace(" ", "").Replace("-", "");

                // ===== VALIDAÇÃO DE EMAIL ÚNICO =====
                if (!string.IsNullOrWhiteSpace(cliente.Email))
                {
                    var emailExiste = await _context.Clientes
                        .AnyAsync(c => c.Email.ToLower() == cliente.Email.ToLower());

                    if (emailExiste)
                    {
                        ModelState.AddModelError("Email", "Já existe um cliente cadastrado com este email.");
                        _logger.LogWarning("Tentativa de cadastro com email duplicado: {Email} por {User}",
                            cliente.Email, User.Identity?.Name);
                    }
                }

                // Validações customizadas antes de verificar ModelState
                await ValidarClienteUnico(cliente);
                ValidarCamposObrigatorios(cliente);

                if (ModelState.IsValid)
                {
                    // Garantir que a data de cadastro seja definida
                    cliente.DataCadastro = DateTime.Now;

                    _context.Add(cliente);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Novo cliente {ClienteNome} (CPF: {ClienteCpf}) criado por {User}",
                        cliente.Nome, cliente.Cpf, User.Identity?.Name);

                    TempData["Sucesso"] = "Cliente cadastrado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // Log dos erros de validação para debug
                    var errors = ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .Select(x => new { Field = x.Key, Errors = x.Value.Errors.Select(e => e.ErrorMessage) });

                    _logger.LogWarning("Validação falhou para cliente. Erros: {Errors}",
                        System.Text.Json.JsonSerializer.Serialize(errors));
                }
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Erro de banco ao criar cliente por {User}", User.Identity?.Name);
                TratarErrosBanco(ex, cliente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao criar cliente por {User}", User.Identity?.Name);
                ModelState.AddModelError(string.Empty, "Erro interno do sistema. Tente novamente.");
            }

            return View(cliente);
        }


        // GET: Clientes/Edit/5 - Apenas Admin e Manager podem editar
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Tentativa de edição de cliente com ID nulo por {User}", User.Identity?.Name);
                return NotFound();
            }

            try
            {
                var cliente = await _context.Clientes.FindAsync(id);
                if (cliente == null)
                {
                    _logger.LogWarning("Tentativa de edição de cliente inexistente {ClienteId} por {User}", id, User.Identity?.Name);
                    return NotFound();
                }

                _logger.LogInformation("Formulário de edição do cliente {ClienteId} acessado por {User}", id, User.Identity?.Name);
                return View(cliente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar formulário de edição do cliente {ClienteId} por {User}", id, User.Identity?.Name);
                TempData["Erro"] = "Erro ao carregar dados do cliente para edição.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Clientes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Cpf,Telefone,Email,Endereco,DataNascimento,NumeroHabilitacao,ValidadeCNH")] Cliente cliente)
        {
            if (id != cliente.Id)
            {
                _logger.LogWarning("Tentativa de edição com ID inconsistente {Id} != {ClienteId} por {User}", id, cliente.Id, User.Identity?.Name);
                return NotFound();
            }

            try
            {
                // AJUSTE: Limpar dados antes da validação
                if (!string.IsNullOrEmpty(cliente.Telefone))
                    cliente.Telefone = cliente.Telefone.Replace("(", "").Replace(")", "").Replace(" ", "").Replace("-", "");

                // Validações customizadas (exclusão do próprio cliente)
                await ValidarClienteUnico(cliente, cliente.Id);
                ValidarCamposObrigatorios(cliente);

                if (ModelState.IsValid)
                {
                    // Preservar a data de cadastro original
                    var clienteOriginal = await _context.Clientes.AsNoTracking().FirstOrDefaultAsync(c => c.Id == cliente.Id);
                    if (clienteOriginal != null)
                    {
                        cliente.DataCadastro = clienteOriginal.DataCadastro;
                    }

                    _context.Update(cliente);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Cliente {ClienteId} ({ClienteNome}) atualizado por {User}",
                        cliente.Id, cliente.Nome, User.Identity?.Name);

                    TempData["Sucesso"] = "Cliente atualizado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // AJUSTE: Log dos erros de validação para debug
                    var errors = ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .Select(x => new { Field = x.Key, Errors = x.Value.Errors.Select(e => e.ErrorMessage) });

                    _logger.LogWarning("Validação falhou para edição do cliente {ClienteId}. Erros: {@Errors}", cliente.Id, errors);
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!ClienteExists(cliente.Id))
                {
                    _logger.LogWarning("Cliente {ClienteId} não existe mais durante edição por {User}", cliente.Id, User.Identity?.Name);
                    return NotFound();
                }
                else
                {
                    _logger.LogError(ex, "Erro de concorrência ao editar cliente {ClienteId} por {User}", cliente.Id, User.Identity?.Name);
                    throw;
                }
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Erro de banco ao editar cliente {ClienteId} por {User}", cliente.Id, User.Identity?.Name);
                TratarErrosBanco(ex, cliente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao editar cliente {ClienteId} por {User}", cliente.Id, User.Identity?.Name);
                ModelState.AddModelError(string.Empty, "Erro interno do sistema. Tente novamente.");
            }

            return View(cliente);
        }

        // GET: Clientes/Delete/5 - Apenas Admin pode excluir
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Tentativa de exclusão de cliente com ID nulo por {User}", User.Identity?.Name);
                return NotFound();
            }

            try
            {
                var cliente = await _context.Clientes
                    .Include(c => c.Locacoes)
                    .Include(c => c.ReservasViagens)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (cliente == null)
                {
                    _logger.LogWarning("Tentativa de exclusão de cliente inexistente {ClienteId} por {User}", id, User.Identity?.Name);
                    return NotFound();
                }

                // Verificar se há impedimentos para exclusão
                var impedimentos = new List<string>();

                if (cliente.Locacoes.Any(l => l.DataDevolucaoReal == null))
                {
                    impedimentos.Add($"{cliente.Locacoes.Count(l => l.DataDevolucaoReal == null)} locação(ões) ativa(s)");
                }

                if (cliente.ReservasViagens.Any(r => r.StatusReservaViagem.Status == "Confirmada" || r.StatusReservaViagem.Status == "Pendente"))
                {
                    var reservasAtivas = await _context.ReservasViagens
                        .Include(r => r.StatusReservaViagem)
                        .Where(r => r.ClienteId == id && (r.StatusReservaViagem.Status == "Confirmada" || r.StatusReservaViagem.Status == "Pendente"))
                        .CountAsync();
                    impedimentos.Add($"{reservasAtivas} reserva(s) ativa(s)");
                }

                if (impedimentos.Any())
                {
                    ViewBag.Impedimentos = impedimentos;
                    _logger.LogInformation("Exclusão do cliente {ClienteId} bloqueada devido a impedimentos: {Impedimentos}",
                        id, string.Join(", ", impedimentos));
                }

                _logger.LogInformation("Formulário de confirmação de exclusão do cliente {ClienteId} acessado por {User}", id, User.Identity?.Name);
                return View(cliente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar formulário de exclusão do cliente {ClienteId} por {User}", id, User.Identity?.Name);
                TempData["Erro"] = "Erro ao carregar dados do cliente para exclusão.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Clientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var cliente = await _context.Clientes
                    .Include(c => c.Locacoes)
                    .Include(c => c.ReservasViagens)
                        .ThenInclude(r => r.StatusReservaViagem)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (cliente != null)
                {
                    // Verificar novamente os impedimentos (segurança extra)
                    var locacoesAtivas = cliente.Locacoes.Where(l => l.DataDevolucaoReal == null).Count();
                    var reservasAtivas = cliente.ReservasViagens
                        .Where(r => r.StatusReservaViagem.Status == "Confirmada" || r.StatusReservaViagem.Status == "Pendente")
                        .Count();

                    if (locacoesAtivas > 0 || reservasAtivas > 0)
                    {
                        var impedimentos = new List<string>();
                        if (locacoesAtivas > 0) impedimentos.Add($"{locacoesAtivas} locação(ões) ativa(s)");
                        if (reservasAtivas > 0) impedimentos.Add($"{reservasAtivas} reserva(s) ativa(s)");

                        TempData["Erro"] = $"Não é possível excluir o cliente. Há {string.Join(" e ", impedimentos)} vinculadas a ele.";
                        _logger.LogWarning("Exclusão do cliente {ClienteId} negada devido a vínculos ativos por {User}", id, User.Identity?.Name);
                        return RedirectToAction(nameof(Delete), new { id = id });
                    }

                    string nomeCliente = cliente.Nome;
                    string cpfCliente = cliente.Cpf;

                    _context.Clientes.Remove(cliente);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Cliente {ClienteId} ({ClienteNome} - {ClienteCpf}) excluído por {User}",
                        id, nomeCliente, cpfCliente, User.Identity?.Name);

                    TempData["Sucesso"] = "Cliente excluído com sucesso!";
                }
                else
                {
                    _logger.LogWarning("Tentativa de exclusão de cliente inexistente {ClienteId} por {User}", id, User.Identity?.Name);
                    TempData["Erro"] = "Cliente não encontrado.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir cliente {ClienteId} por {User}", id, User.Identity?.Name);
                TempData["Erro"] = "Erro ao excluir cliente. Tente novamente.";
            }

            return RedirectToAction(nameof(Index));
        }

        // ========== MÉTODOS AUXILIARES ==========

        private bool ClienteExists(int id)
        {
            return _context.Clientes.Any(e => e.Id == id);
        }

        private async Task ValidarClienteUnico(Cliente cliente, int? idExcluir = null)
        {
            // Validar CPF único
            var cpfExistente = await _context.Clientes
                .AnyAsync(c => c.Cpf == cliente.Cpf && c.Id != idExcluir);

            if (cpfExistente)
            {
                ModelState.AddModelError("Cpf", "Já existe um cliente cadastrado com este CPF.");
                _logger.LogWarning("Tentativa de cadastro com CPF duplicado: {Cpf}", cliente.Cpf);
            }

            // ===== CORREÇÃO: Validar email único com comparação case-insensitive =====
            if (!string.IsNullOrWhiteSpace(cliente.Email))
            {
                var emailExistente = await _context.Clientes
                    .AnyAsync(c => c.Email.ToLower() == cliente.Email.ToLower() && c.Id != idExcluir);

                if (emailExistente)
                {
                    ModelState.AddModelError("Email", "Já existe um cliente cadastrado com este email.");
                    _logger.LogWarning("Tentativa de cadastro com email duplicado: {Email}", cliente.Email);
                }
            }
        }

        private void ValidarCamposObrigatorios(Cliente cliente)
        {
            // AJUSTE: Melhorar validação do telefone
            if (string.IsNullOrWhiteSpace(cliente.Telefone))
            {
                ModelState.AddModelError("Telefone", "O telefone é obrigatório.");
            }
            else
            {
                // Validar formato do telefone
                var numeros = cliente.Telefone.Replace("(", "").Replace(")", "").Replace(" ", "").Replace("-", "");
                if (numeros.Length < 10 || numeros.Length > 11)
                {
                    ModelState.AddModelError("Telefone", "Telefone deve ter 10 ou 11 dígitos.");
                }
            }

            // AJUSTE: Melhorar validação do email
            if (string.IsNullOrWhiteSpace(cliente.Email))
            {
                ModelState.AddModelError("Email", "O email é obrigatório.");
            }
            else if (!IsValidEmail(cliente.Email))
            {
                ModelState.AddModelError("Email", "Email deve ter um formato válido.");
            }

            // Validação de idade mínima (CORRIGIR: era 21, mas modelo diz 18+)
            var idade = DateTime.Now.Year - cliente.DataNascimento.Year;
            if (DateTime.Now.DayOfYear < cliente.DataNascimento.DayOfYear)
                idade--;

            if (idade < 21) // Manter 21 como regra de negócio
            {
                ModelState.AddModelError("DataNascimento", "O cliente deve ter pelo menos 21 anos.");
            }
            else if (idade > 120)
            {
                ModelState.AddModelError("DataNascimento", "Idade inválida.");
            }

            // AJUSTE: Melhorar validação de CNH
            if (!string.IsNullOrWhiteSpace(cliente.NumeroHabilitacao))
            {
                if (!cliente.ValidadeCNH.HasValue)
                {
                    ModelState.AddModelError("ValidadeCNH", "Informe a data de validade da CNH.");
                }
                else if (cliente.ValidadeCNH.Value.Date < DateTime.Now.Date)
                {
                    ModelState.AddModelError("ValidadeCNH", "A CNH está vencida.");
                }
            }

            // Se tem data de validade mas não tem número
            if (cliente.ValidadeCNH.HasValue && string.IsNullOrWhiteSpace(cliente.NumeroHabilitacao))
            {
                ModelState.AddModelError("NumeroHabilitacao", "Informe o número da CNH.");
            }
        }

        private static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        // API: Validar email único
        [HttpGet]
        public async Task<IActionResult> ValidarEmailUnico(string email, int? id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    return Json(new { valido = true });
                }

                var emailExiste = await _context.Clientes
                    .AnyAsync(c => c.Email.ToLower() == email.ToLower() && c.Id != id);

                return Json(new
                {
                    valido = !emailExiste,
                    mensagem = emailExiste ? "Este email já está cadastrado." : null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar email único");
                return Json(new { valido = true }); // Em caso de erro, permite continuar
            }
        }

        private void TratarErrosBanco(DbUpdateException ex, Cliente cliente)
        {
            var errorMessage = ex.InnerException?.Message ?? ex.Message;

            if (errorMessage.Contains("CPF") || errorMessage.Contains("IX_Clientes_Cpf"))
            {
                ModelState.AddModelError("Cpf", "Já existe um cliente com este CPF.");
            }
            else if (errorMessage.Contains("Email") || errorMessage.Contains("IX_Clientes_Email"))
            {
                ModelState.AddModelError("Email", "Já existe um cliente com este email.");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Erro ao salvar no banco de dados. Verifique os dados e tente novamente.");

                // Log detalhado para debug
                _logger.LogError("Erro de banco não tratado: {ErrorMessage}", errorMessage);
            }
        }

        // ========== APIs PARA CONSUMO INTERNO (AJAX) ==========

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetClienteData(int id)
        {
            try
            {
                var cliente = await _context.Clientes
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (cliente == null)
                {
                    return NotFound(new { message = "Cliente não encontrado" });
                }

                var resultado = new
                {
                    id = cliente.Id,
                    nome = cliente.Nome,
                    cpf = cliente.Cpf,
                    email = cliente.Email,
                    telefone = cliente.Telefone,
                    numeroHabilitacao = cliente.NumeroHabilitacao,
                    validadeCNH = cliente.ValidadeCNH?.ToString("dd/MM/yyyy"),
                    cnhValida = !string.IsNullOrWhiteSpace(cliente.NumeroHabilitacao) &&
                               cliente.ValidadeCNH.HasValue &&
                               cliente.ValidadeCNH.Value.Date >= DateTime.Now.Date,
                    idade = DateTime.Now.Year - cliente.DataNascimento.Year -
                           (DateTime.Now.DayOfYear < cliente.DataNascimento.DayOfYear ? 1 : 0)
                };

                return Json(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter dados do cliente {ClienteId} via API por {User}", id, User.Identity?.Name);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ValidarCNH(int clienteId)
        {
            try
            {
                var cliente = await _context.Clientes
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == clienteId);

                if (cliente == null)
                {
                    return NotFound(new { message = "Cliente não encontrado" });
                }

                var hoje = DateTime.Now.Date;
                bool cnhValida = !string.IsNullOrWhiteSpace(cliente.NumeroHabilitacao) &&
                                cliente.ValidadeCNH.HasValue &&
                                cliente.ValidadeCNH.Value.Date >= hoje;

                string mensagem;
                if (string.IsNullOrWhiteSpace(cliente.NumeroHabilitacao))
                {
                    mensagem = "Cliente não possui número de habilitação cadastrado.";
                }
                else if (!cliente.ValidadeCNH.HasValue)
                {
                    mensagem = "Cliente não possui data de validade da CNH cadastrada.";
                }
                else if (cliente.ValidadeCNH.Value.Date < hoje)
                {
                    mensagem = $"CNH vencida em {cliente.ValidadeCNH.Value:dd/MM/yyyy}.";
                }
                else
                {
                    var diasRestantes = (cliente.ValidadeCNH.Value.Date - hoje).Days;
                    if (diasRestantes <= 30)
                    {
                        mensagem = $"CNH válida, mas vence em {diasRestantes} dias ({cliente.ValidadeCNH.Value:dd/MM/yyyy}).";
                    }
                    else
                    {
                        mensagem = $"CNH válida até {cliente.ValidadeCNH.Value:dd/MM/yyyy}.";
                    }
                }

                return Json(new
                {
                    valida = cnhValida,
                    mensagem = mensagem,
                    dataVencimento = cliente.ValidadeCNH?.ToString("dd/MM/yyyy")
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar CNH do cliente {ClienteId} via API por {User}", clienteId, User.Identity?.Name);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        // Busca rápida de clientes para autocomplete
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> BuscarClientes(string termo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(termo) || termo.Length < 2)
                {
                    return Json(new List<object>());
                }

                var clientes = await _context.Clientes
                    .Where(c => c.Nome.Contains(termo) || c.Cpf.Contains(termo))
                    .Select(c => new
                    {
                        id = c.Id,
                        nome = c.Nome,
                        cpf = c.Cpf,
                        email = c.Email,
                        telefone = c.Telefone
                    })
                    .Take(10)
                    .ToListAsync();

                return Json(clientes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro na busca de clientes com termo '{Termo}' por {User}", termo, User.Identity?.Name);
                return Json(new List<object>());
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> BuscarClientesAvancada(string termo, int? limit = 10)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(termo) || termo.Length < 2)
                {
                    return Json(new { success = false, message = "Digite pelo menos 2 caracteres" });
                }

                var termoBusca = termo.Trim().ToLower();

                var clientes = await _context.Clientes
                    .Where(c => c.Nome.ToLower().Contains(termoBusca) ||
                               c.Cpf.Contains(termoBusca) ||
                               (c.Email != null && c.Email.ToLower().Contains(termoBusca)))
                    .Select(c => new
                    {
                        id = c.Id,
                        nome = c.Nome,
                        cpf = c.Cpf,
                        email = c.Email,
                        telefone = c.Telefone,
                        cnhValida = !string.IsNullOrEmpty(c.NumeroHabilitacao) &&
                                   c.ValidadeCNH.HasValue &&
                                   c.ValidadeCNH.Value.Date >= DateTime.Now.Date
                    })
                    .Take(limit ?? 10)
                    .ToListAsync();

                return Json(new { success = true, data = clientes, total = clientes.Count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro na busca avançada de clientes com termo '{Termo}' por {User}", termo, User.Identity?.Name);
                return Json(new { success = false, message = "Erro interno na busca" });
            }
        }
    }
}