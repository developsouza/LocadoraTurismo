using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RentalTourismSystem.Data;
using RentalTourismSystem.Models;

namespace RentalTourismSystem.Controllers
{
    [Authorize] // Todo o controller requer autenticação
    public class FuncionariosController : Controller
    {
        private readonly RentalTourismContext _context;
        private readonly ILogger<FuncionariosController> _logger;

        public FuncionariosController(RentalTourismContext context, ILogger<FuncionariosController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Funcionarios - Todos podem ver (diferentes níveis de informação)
        public async Task<IActionResult> Index(int? agenciaId, string? cargo, string? busca)
        {
            try
            {
                _logger.LogInformation("Lista de funcionários acessada por usuário {User}", User.Identity?.Name);

                var funcionarios = _context.Funcionarios
                    .Include(f => f.Agencia)
                    .AsQueryable();

                // Filtros
                if (agenciaId.HasValue)
                {
                    funcionarios = funcionarios.Where(f => f.AgenciaId == agenciaId);
                }

                if (!string.IsNullOrEmpty(cargo))
                {
                    funcionarios = funcionarios.Where(f => f.Cargo.Contains(cargo));
                }

                if (!string.IsNullOrEmpty(busca))
                {
                    funcionarios = funcionarios.Where(f => f.Nome.Contains(busca) ||
                                                         f.Cpf.Contains(busca) ||
                                                         f.Email.Contains(busca));
                }

                ViewBag.AgenciaId = new SelectList(await _context.Agencias.ToListAsync(), "Id", "Nome", agenciaId);
                ViewBag.Cargo = cargo;
                ViewBag.Busca = busca;

                var listaFuncionarios = await funcionarios.OrderBy(f => f.Nome).ToListAsync();

                // Funcionários podem ver apenas informações básicas dos colegas
                if (!User.IsInRole("Admin") && !User.IsInRole("Manager"))
                {
                    // Ocultar informações sensíveis como salário para funcionários comuns
                    foreach (var func in listaFuncionarios)
                    {
                        func.Salario = 0; // Zerar salário na exibição
                    }
                }

                return View(listaFuncionarios);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar lista de funcionários para usuário {User}", User.Identity?.Name);
                TempData["Erro"] = "Erro ao carregar lista de funcionários. Tente novamente.";
                ViewBag.AgenciaId = new SelectList(new List<Agencia>(), "Id", "Nome");
                return View(new List<Funcionario>());
            }
        }

        // GET: Funcionarios/Details/5 - Níveis diferentes de detalhes por role
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Tentativa de acesso a detalhes de funcionário com ID nulo por {User}", User.Identity?.Name);
                return NotFound();
            }

            try
            {
                var funcionario = await _context.Funcionarios
                    .Include(f => f.Agencia)
                    .Include(f => f.Locacoes.OrderByDescending(l => l.DataRetirada).Take(10))
                        .ThenInclude(l => l.Cliente)
                    .Include(f => f.Locacoes)
                        .ThenInclude(l => l.Veiculo)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (funcionario == null)
                {
                    _logger.LogWarning("Funcionário com ID {FuncionarioId} não encontrado. Acessado por {User}", id, User.Identity?.Name);
                    return NotFound();
                }

                // Calcular estatísticas do funcionário
                var hoje = DateTime.Now;
                var inicioMes = new DateTime(hoje.Year, hoje.Month, 1);

                ViewBag.VendasMes = funcionario.Locacoes
                    .Where(l => l.DataRetirada >= inicioMes)
                    .Count();

                ViewBag.ReceitaMes = funcionario.Locacoes
                    .Where(l => l.DataRetirada >= inicioMes)
                    .Sum(l => l.ValorTotal);

                ViewBag.TotalVendas = funcionario.Locacoes.Count();

                ViewBag.ReceitaTotal = funcionario.Locacoes.Sum(l => l.ValorTotal);

                // Funcionários comuns não podem ver salário de outros
                if (!User.IsInRole("Admin") && !User.IsInRole("Manager"))
                {
                    funcionario.Salario = 0;
                }

                _logger.LogInformation("Detalhes do funcionário {FuncionarioId} acessados por {User}", id, User.Identity?.Name);
                return View(funcionario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar detalhes do funcionário {FuncionarioId} para usuário {User}", id, User.Identity?.Name);
                TempData["Erro"] = "Erro ao carregar dados do funcionário.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Funcionarios/Create - Apenas Admin e Manager podem criar
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create()
        {
            try
            {
                ViewBag.AgenciaId = new SelectList(await _context.Agencias.OrderBy(a => a.Nome).ToListAsync(), "Id", "Nome");
                _logger.LogInformation("Formulário de criação de funcionário acessado por {User}", User.Identity?.Name);
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar formulário de criação de funcionário por {User}", User.Identity?.Name);
                TempData["Erro"] = "Erro ao carregar formulário.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Funcionarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create([Bind("Nome,Cpf,Telefone,Email,Cargo,Salario,DataAdmissao,AgenciaId")] Funcionario funcionario)
        {
            try
            {
                ModelState.Remove("Agencia");

                // Validar funcionário único
                await ValidarFuncionarioUnico(funcionario);

                if (ModelState.IsValid)
                {
                    _context.Add(funcionario);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Novo funcionário '{FuncionarioNome}' (CPF: {FuncionarioCpf}) criado por {User}",
                        funcionario.Nome, funcionario.Cpf, User.Identity?.Name);

                    TempData["Sucesso"] = "Funcionário cadastrado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Erro de banco ao criar funcionário por {User}", User.Identity?.Name);
                TratarErrosBanco(ex, funcionario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao criar funcionário por {User}", User.Identity?.Name);
                ModelState.AddModelError(string.Empty, "Erro interno do sistema. Tente novamente.");
            }

            ViewBag.AgenciaId = new SelectList(await _context.Agencias.ToListAsync(), "Id", "Nome", funcionario.AgenciaId);
            return View(funcionario);
        }

        // GET: Funcionarios/Edit/5 - Apenas Admin pode editar
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Tentativa de edição de funcionário com ID nulo por {User}", User.Identity?.Name);
                return NotFound();
            }

            try
            {
                var funcionario = await _context.Funcionarios
                    .Include(f => f.Agencia)
                    .FirstOrDefaultAsync(f => f.Id == id);

                if (funcionario == null)
                {
                    _logger.LogWarning("Tentativa de edição de funcionário inexistente {FuncionarioId} por {User}", id, User.Identity?.Name);
                    return NotFound();
                }

                ViewBag.AgenciaId = new SelectList(await _context.Agencias.ToListAsync(), "Id", "Nome", funcionario.AgenciaId);
                _logger.LogInformation("Formulário de edição do funcionário {FuncionarioId} acessado por {User}", id, User.Identity?.Name);
                return View(funcionario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar formulário de edição do funcionário {FuncionarioId} por {User}", id, User.Identity?.Name);
                TempData["Erro"] = "Erro ao carregar dados do funcionário para edição.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Funcionarios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Cpf,Telefone,Email,Cargo,Salario,DataAdmissao,AgenciaId")] Funcionario funcionario)
        {
            if (id != funcionario.Id)
            {
                _logger.LogWarning("Tentativa de edição com ID inconsistente {Id} != {FuncionarioId} por {User}",
                    id, funcionario.Id, User.Identity?.Name);
                return NotFound();
            }

            try
            {
                ModelState.Remove("Agencia");

                // Validar funcionário único (excluindo o próprio funcionário)
                await ValidarFuncionarioUnico(funcionario, funcionario.Id);

                if (ModelState.IsValid)
                {
                    _context.Update(funcionario);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Funcionário {FuncionarioId} ('{FuncionarioNome}') atualizado por {User}",
                        funcionario.Id, funcionario.Nome, User.Identity?.Name);

                    TempData["Sucesso"] = "Funcionário atualizado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!FuncionarioExists(funcionario.Id))
                {
                    _logger.LogWarning("Funcionário {FuncionarioId} não existe mais durante edição por {User}", funcionario.Id, User.Identity?.Name);
                    return NotFound();
                }
                else
                {
                    _logger.LogError(ex, "Erro de concorrência ao editar funcionário {FuncionarioId} por {User}", funcionario.Id, User.Identity?.Name);
                    throw;
                }
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Erro de banco ao editar funcionário {FuncionarioId} por {User}", funcionario.Id, User.Identity?.Name);
                TratarErrosBanco(ex, funcionario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao editar funcionário {FuncionarioId} por {User}", funcionario.Id, User.Identity?.Name);
                ModelState.AddModelError(string.Empty, "Erro interno do sistema. Tente novamente.");
            }

            ViewBag.AgenciaId = new SelectList(await _context.Agencias.ToListAsync(), "Id", "Nome", funcionario.AgenciaId);
            return View(funcionario);
        }

        // GET: Funcionarios/Delete/5 - Apenas Admin pode excluir
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Tentativa de exclusão de funcionário com ID nulo por {User}", User.Identity?.Name);
                return NotFound();
            }

            try
            {
                var funcionario = await _context.Funcionarios
                    .Include(f => f.Agencia)
                    .Include(f => f.Locacoes)
                        .ThenInclude(l => l.Cliente)
                    .Include(f => f.Locacoes)
                        .ThenInclude(l => l.Veiculo)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (funcionario == null)
                {
                    _logger.LogWarning("Tentativa de exclusão de funcionário inexistente {FuncionarioId} por {User}", id, User.Identity?.Name);
                    return NotFound();
                }

                // Verificar se há impedimentos para exclusão
                var impedimentos = new List<string>();

                var locacoesAtivas = funcionario.Locacoes.Where(l => l.DataDevolucaoReal == null).Count();
                if (locacoesAtivas > 0)
                {
                    impedimentos.Add($"{locacoesAtivas} locação(ões) ativa(s)");
                }

                var totalLocacoes = funcionario.Locacoes.Count;
                if (totalLocacoes > 0)
                {
                    impedimentos.Add($"{totalLocacoes} locação(ões) no histórico");
                }

                if (impedimentos.Any())
                {
                    ViewBag.Impedimentos = impedimentos;
                    _logger.LogInformation("Exclusão do funcionário {FuncionarioId} bloqueada devido a impedimentos: {Impedimentos}",
                        id, string.Join(", ", impedimentos));
                }

                _logger.LogInformation("Formulário de confirmação de exclusão do funcionário {FuncionarioId} acessado por {User}", id, User.Identity?.Name);
                return View(funcionario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar formulário de exclusão do funcionário {FuncionarioId} por {User}", id, User.Identity?.Name);
                TempData["Erro"] = "Erro ao carregar dados do funcionário para exclusão.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Funcionarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var funcionario = await _context.Funcionarios
                    .Include(f => f.Locacoes)
                    .FirstOrDefaultAsync(f => f.Id == id);

                if (funcionario != null)
                {
                    // Verificar novamente os impedimentos (segurança extra)
                    var locacoesAtivas = funcionario.Locacoes.Where(l => l.DataDevolucaoReal == null).Count();

                    if (locacoesAtivas > 0)
                    {
                        TempData["Erro"] = $"Não é possível excluir o funcionário. Existem {locacoesAtivas} locação(ões) ativa(s) vinculada(s) a ele.";
                        _logger.LogWarning("Exclusão do funcionário {FuncionarioId} negada devido a locações ativas por {User}", id, User.Identity?.Name);
                        return RedirectToAction(nameof(Delete), new { id = id });
                    }

                    string nomeFuncionario = funcionario.Nome;
                    string cpfFuncionario = funcionario.Cpf;

                    _context.Funcionarios.Remove(funcionario);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Funcionário {FuncionarioId} ('{FuncionarioNome}' - {FuncionarioCpf}) excluído por {User}",
                        id, nomeFuncionario, cpfFuncionario, User.Identity?.Name);

                    TempData["Sucesso"] = "Funcionário excluído com sucesso!";
                }
                else
                {
                    _logger.LogWarning("Tentativa de exclusão de funcionário inexistente {FuncionarioId} por {User}", id, User.Identity?.Name);
                    TempData["Erro"] = "Funcionário não encontrado.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir funcionário {FuncionarioId} por {User}", id, User.Identity?.Name);
                TempData["Erro"] = "Erro ao excluir funcionário. Tente novamente.";
            }

            return RedirectToAction(nameof(Index));
        }

        // ========== RELATÓRIO DE PERFORMANCE (Apenas Admin e Manager) ==========

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Performance(DateTime? dataInicio, DateTime? dataFim)
        {
            try
            {
                // Definir período padrão (último mês)
                dataInicio ??= DateTime.Now.AddMonths(-1);
                dataFim ??= DateTime.Now;

                var funcionarios = await _context.Funcionarios
                    .Include(f => f.Agencia)
                    .Include(f => f.Locacoes)
                        .ThenInclude(l => l.Cliente)
                    .ToListAsync();

                var performanceList = funcionarios.Select(f => new
                {
                    Funcionario = f,
                    LocacoesPeriodo = f.Locacoes.Where(l => l.DataRetirada >= dataInicio && l.DataRetirada <= dataFim).Count(),
                    ReceitaPeriodo = f.Locacoes.Where(l => l.DataRetirada >= dataInicio && l.DataRetirada <= dataFim).Sum(l => l.ValorTotal),
                    TicketMedio = f.Locacoes.Where(l => l.DataRetirada >= dataInicio && l.DataRetirada <= dataFim).Any() ?
                        f.Locacoes.Where(l => l.DataRetirada >= dataInicio && l.DataRetirada <= dataFim).Average(l => l.ValorTotal) : 0
                })
                .OrderByDescending(p => p.ReceitaPeriodo)
                .ToList();

                ViewBag.DataInicio = dataInicio;
                ViewBag.DataFim = dataFim;
                ViewBag.PerformanceList = performanceList;

                _logger.LogInformation("Relatório de performance de funcionários acessado por {User} - Período: {DataInicio} a {DataFim}",
                    User.Identity?.Name, dataInicio?.ToString("dd/MM/yyyy"), dataFim?.ToString("dd/MM/yyyy"));

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar relatório de performance por {User}", User.Identity?.Name);
                TempData["Erro"] = "Erro ao gerar relatório de performance.";
                return RedirectToAction(nameof(Index));
            }
        }

        // ========== MÉTODOS AUXILIARES ==========

        private bool FuncionarioExists(int id)
        {
            return _context.Funcionarios.Any(e => e.Id == id);
        }

        private async Task ValidarFuncionarioUnico(Funcionario funcionario, int? idExcluir = null)
        {
            // Validar CPF único
            var cpfExistente = await _context.Funcionarios
                .AnyAsync(f => f.Cpf == funcionario.Cpf && f.Id != idExcluir);

            if (cpfExistente)
            {
                ModelState.AddModelError("Cpf", "Já existe um funcionário cadastrado com este CPF.");
            }

            // Validar email único (se informado)
            if (!string.IsNullOrWhiteSpace(funcionario.Email))
            {
                var emailExistente = await _context.Funcionarios
                    .AnyAsync(f => f.Email == funcionario.Email && f.Id != idExcluir);

                if (emailExistente)
                {
                    ModelState.AddModelError("Email", "Já existe um funcionário cadastrado com este email.");
                }
            }
        }

        private void TratarErrosBanco(DbUpdateException ex, Funcionario funcionario)
        {
            if (ex.InnerException != null && ex.InnerException.Message.Contains("CPF"))
            {
                ModelState.AddModelError("Cpf", "Já existe um funcionário com este CPF.");
            }
            else if (ex.InnerException != null && ex.InnerException.Message.Contains("Email"))
            {
                ModelState.AddModelError("Email", "Já existe um funcionário com este email.");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Erro ao salvar no banco de dados. Verifique os dados e tente novamente.");
            }
        }

        // ========== APIs PARA CONSUMO INTERNO (AJAX) ==========

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetFuncionarioData(int id)
        {
            try
            {
                var funcionario = await _context.Funcionarios
                    .Include(f => f.Agencia)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(f => f.Id == id);

                if (funcionario == null)
                {
                    return NotFound(new { message = "Funcionário não encontrado" });
                }

                var resultado = new
                {
                    id = funcionario.Id,
                    nome = funcionario.Nome,
                    cpf = funcionario.Cpf,
                    email = funcionario.Email,
                    telefone = funcionario.Telefone,
                    cargo = funcionario.Cargo,
                    agencia = funcionario.Agencia?.Nome,
                    agenciaId = funcionario.AgenciaId,
                    dataAdmissao = funcionario.DataAdmissao.ToString("dd/MM/yyyy"),
                    // Salário só é retornado para Admin e Manager
                    salario = (User.IsInRole("Admin") || User.IsInRole("Manager")) ? funcionario.Salario : 0
                };

                return Json(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter dados do funcionário {FuncionarioId} via API por {User}", id, User.Identity?.Name);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        // Busca rápida de funcionários para autocomplete
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> BuscarFuncionarios(string? termo = null)
        {
            try
            {
                var query = _context.Funcionarios
                    .Include(f => f.Agencia)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(termo))
                {
                    query = query.Where(f => f.Nome.Contains(termo) || f.Cpf.Contains(termo));
                }

                var funcionarios = await query
                    .Select(f => new
                    {
                        id = f.Id,
                        nome = f.Nome,
                        cpf = f.Cpf,
                        cargo = f.Cargo,
                        agencia = f.Agencia.Nome,
                        descricao = $"{f.Nome} - {f.Cargo} ({f.Agencia.Nome})"
                    })
                    .Take(10)
                    .ToListAsync();

                return Json(funcionarios);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro na busca de funcionários por {User}", User.Identity?.Name);
                return Json(new List<object>());
            }
        }

        // Obter funcionários por agência
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetFuncionariosPorAgencia(int agenciaId)
        {
            try
            {
                var funcionarios = await _context.Funcionarios
                    .Where(f => f.AgenciaId == agenciaId)
                    .Select(f => new
                    {
                        id = f.Id,
                        nome = f.Nome,
                        cargo = f.Cargo
                    })
                    .OrderBy(f => f.nome)
                    .ToListAsync();

                return Json(funcionarios);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter funcionários da agência {AgenciaId} por {User}", agenciaId, User.Identity?.Name);
                return Json(new List<object>());
            }
        }

        // Estatísticas do funcionário
        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetEstatisticasFuncionario(int id, DateTime? dataInicio = null, DateTime? dataFim = null)
        {
            try
            {
                dataInicio ??= DateTime.Now.AddMonths(-1);
                dataFim ??= DateTime.Now;

                var funcionario = await _context.Funcionarios
                    .Include(f => f.Locacoes)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(f => f.Id == id);

                if (funcionario == null)
                {
                    return NotFound();
                }

                var locacoesPeriodo = funcionario.Locacoes
                    .Where(l => l.DataRetirada >= dataInicio && l.DataRetirada <= dataFim)
                    .ToList();

                var estatisticas = new
                {
                    vendas = locacoesPeriodo.Count,
                    receita = locacoesPeriodo.Sum(l => l.ValorTotal),
                    ticketMedio = locacoesPeriodo.Any() ? locacoesPeriodo.Average(l => l.ValorTotal) : 0,
                    totalVendas = funcionario.Locacoes.Count,
                    receitaTotal = funcionario.Locacoes.Sum(l => l.ValorTotal)
                };

                return Json(estatisticas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter estatísticas do funcionário {FuncionarioId} por {User}", id, User.Identity?.Name);
                return Json(new { error = "Erro ao carregar estatísticas" });
            }
        }
    }
}