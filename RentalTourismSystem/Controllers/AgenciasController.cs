using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentalTourismSystem.Data;
using RentalTourismSystem.Models;

namespace RentalTourismSystem.Controllers
{
    [Authorize] // Todo o controller requer autenticação
    public class AgenciasController : Controller
    {
        private readonly RentalTourismContext _context;
        private readonly ILogger<AgenciasController> _logger;

        public AgenciasController(RentalTourismContext context, ILogger<AgenciasController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Agencias - Todos os funcionários podem ver
        public async Task<IActionResult> Index()
        {
            try
            {
                _logger.LogInformation("Lista de agências acessada por usuário {User}", User.Identity?.Name);

                var agencias = await _context.Agencias
                    .Include(a => a.Funcionarios)
                    .Include(a => a.Veiculos)
                    .Include(a => a.Locacoes)
                    .OrderBy(a => a.Nome)
                    .ToListAsync();

                return View(agencias);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar lista de agências para usuário {User}", User.Identity?.Name);
                TempData["Erro"] = "Erro ao carregar lista de agências. Tente novamente.";
                return View(new List<Agencia>());
            }
        }

        // GET: Agencias/Details/5 - Todos podem ver detalhes
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Tentativa de acesso a detalhes de agência com ID nulo por {User}", User.Identity?.Name);
                return NotFound();
            }

            try
            {
                var agencia = await _context.Agencias
                    .Include(a => a.Funcionarios)
                    .Include(a => a.Veiculos)
                        .ThenInclude(v => v.StatusCarro)
                    .Include(a => a.Locacoes.OrderByDescending(l => l.DataRetirada).Take(10))
                        .ThenInclude(l => l.Cliente)
                    .Include(a => a.Locacoes)
                        .ThenInclude(l => l.Veiculo)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (agencia == null)
                {
                    _logger.LogWarning("Agência com ID {AgenciaId} não encontrada. Acessado por {User}", id, User.Identity?.Name);
                    return NotFound();
                }

                // Calcular estatísticas da agência
                var hoje = DateTime.Now;
                var inicioMes = new DateTime(hoje.Year, hoje.Month, 1);

                ViewBag.LocacoesMes = agencia.Locacoes
                    .Where(l => l.DataRetirada >= inicioMes)
                    .Count();

                ViewBag.ReceitaMes = agencia.Locacoes
                    .Where(l => l.DataRetirada >= inicioMes)
                    .Sum(l => l.ValorTotal);

                ViewBag.VeiculosDisponiveis = agencia.Veiculos
                    .Where(v => v.StatusCarro.Status == "Disponível")
                    .Count();

                ViewBag.LocacoesAtivas = agencia.Locacoes
                    .Where(l => l.DataDevolucaoReal == null)
                    .Count();

                _logger.LogInformation("Detalhes da agência {AgenciaId} acessados por {User}", id, User.Identity?.Name);
                return View(agencia);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar detalhes da agência {AgenciaId} para usuário {User}", id, User.Identity?.Name);
                TempData["Erro"] = "Erro ao carregar dados da agência.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Agencias/Create - Apenas Admin pode criar
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            _logger.LogInformation("Formulário de criação de agência acessado por {User}", User.Identity?.Name);
            return View();
        }

        // POST: Agencias/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Nome,Endereco,Telefone,Email")] Agencia agencia)
        {
            try
            {
                // Validar nome único
                await ValidarAgenciaUnica(agencia);

                if (ModelState.IsValid)
                {
                    _context.Add(agencia);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Nova agência '{AgenciaNome}' criada por {User}", agencia.Nome, User.Identity?.Name);

                    TempData["Sucesso"] = "Agência criada com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Erro de banco ao criar agência por {User}", User.Identity?.Name);
                TratarErrosBanco(ex, agencia);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao criar agência por {User}", User.Identity?.Name);
                ModelState.AddModelError(string.Empty, "Erro interno do sistema. Tente novamente.");
            }

            return View(agencia);
        }

        // GET: Agencias/Edit/5 - Apenas Admin pode editar
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Tentativa de edição de agência com ID nulo por {User}", User.Identity?.Name);
                return NotFound();
            }

            try
            {
                var agencia = await _context.Agencias.FindAsync(id);
                if (agencia == null)
                {
                    _logger.LogWarning("Tentativa de edição de agência inexistente {AgenciaId} por {User}", id, User.Identity?.Name);
                    return NotFound();
                }

                _logger.LogInformation("Formulário de edição da agência {AgenciaId} acessado por {User}", id, User.Identity?.Name);
                return View(agencia);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar formulário de edição da agência {AgenciaId} por {User}", id, User.Identity?.Name);
                TempData["Erro"] = "Erro ao carregar dados da agência para edição.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Agencias/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Endereco,Telefone,Email")] Agencia agencia)
        {
            if (id != agencia.Id)
            {
                _logger.LogWarning("Tentativa de edição com ID inconsistente {Id} != {AgenciaId} por {User}",
                    id, agencia.Id, User.Identity?.Name);
                return NotFound();
            }

            try
            {
                // Validar nome único (excluindo a própria agência)
                await ValidarAgenciaUnica(agencia, agencia.Id);

                if (ModelState.IsValid)
                {
                    _context.Update(agencia);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Agência {AgenciaId} ('{AgenciaNome}') atualizada por {User}",
                        agencia.Id, agencia.Nome, User.Identity?.Name);

                    TempData["Sucesso"] = "Agência atualizada com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!AgenciaExists(agencia.Id))
                {
                    _logger.LogWarning("Agência {AgenciaId} não existe mais durante edição por {User}", agencia.Id, User.Identity?.Name);
                    return NotFound();
                }
                else
                {
                    _logger.LogError(ex, "Erro de concorrência ao editar agência {AgenciaId} por {User}", agencia.Id, User.Identity?.Name);
                    throw;
                }
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Erro de banco ao editar agência {AgenciaId} por {User}", agencia.Id, User.Identity?.Name);
                TratarErrosBanco(ex, agencia);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao editar agência {AgenciaId} por {User}", agencia.Id, User.Identity?.Name);
                ModelState.AddModelError(string.Empty, "Erro interno do sistema. Tente novamente.");
            }

            return View(agencia);
        }

        // GET: Agencias/Delete/5 - Apenas Admin pode excluir
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Tentativa de exclusão de agência com ID nulo por {User}", User.Identity?.Name);
                return NotFound();
            }

            try
            {
                var agencia = await _context.Agencias
                    .Include(a => a.Funcionarios)
                    .Include(a => a.Veiculos)
                    .Include(a => a.Locacoes)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (agencia == null)
                {
                    _logger.LogWarning("Tentativa de exclusão de agência inexistente {AgenciaId} por {User}", id, User.Identity?.Name);
                    return NotFound();
                }

                // Verificar se há impedimentos para exclusão
                var impedimentos = new List<string>();

                if (agencia.Funcionarios.Any())
                {
                    impedimentos.Add($"{agencia.Funcionarios.Count} funcionário(s) vinculado(s)");
                }

                if (agencia.Veiculos.Any())
                {
                    impedimentos.Add($"{agencia.Veiculos.Count} veículo(s) vinculado(s)");
                }

                if (agencia.Locacoes.Any())
                {
                    impedimentos.Add($"{agencia.Locacoes.Count} locação(ões) no histórico");
                }

                if (impedimentos.Any())
                {
                    ViewBag.Impedimentos = impedimentos;
                    _logger.LogInformation("Exclusão da agência {AgenciaId} bloqueada devido a impedimentos: {Impedimentos}",
                        id, string.Join(", ", impedimentos));
                }

                _logger.LogInformation("Formulário de confirmação de exclusão da agência {AgenciaId} acessado por {User}", id, User.Identity?.Name);
                return View(agencia);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar formulário de exclusão da agência {AgenciaId} por {User}", id, User.Identity?.Name);
                TempData["Erro"] = "Erro ao carregar dados da agência para exclusão.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Agencias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var agencia = await _context.Agencias
                    .Include(a => a.Funcionarios)
                    .Include(a => a.Veiculos)
                    .Include(a => a.Locacoes)
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (agencia != null)
                {
                    // Verificar novamente os impedimentos (segurança extra)
                    var impedimentos = new List<string>();

                    if (agencia.Funcionarios.Any())
                        impedimentos.Add($"{agencia.Funcionarios.Count} funcionário(s)");

                    if (agencia.Veiculos.Any())
                        impedimentos.Add($"{agencia.Veiculos.Count} veículo(s)");

                    if (agencia.Locacoes.Any())
                        impedimentos.Add($"{agencia.Locacoes.Count} locação(ões)");

                    if (impedimentos.Any())
                    {
                        TempData["Erro"] = $"Não é possível excluir a agência. Há {string.Join(", ", impedimentos)} vinculado(s) a ela.";
                        _logger.LogWarning("Exclusão da agência {AgenciaId} negada devido a vínculos por {User}", id, User.Identity?.Name);
                        return RedirectToAction(nameof(Delete), new { id = id });
                    }

                    string nomeAgencia = agencia.Nome;

                    _context.Agencias.Remove(agencia);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Agência {AgenciaId} ('{AgenciaNome}') excluída por {User}",
                        id, nomeAgencia, User.Identity?.Name);

                    TempData["Sucesso"] = "Agência excluída com sucesso!";
                }
                else
                {
                    _logger.LogWarning("Tentativa de exclusão de agência inexistente {AgenciaId} por {User}", id, User.Identity?.Name);
                    TempData["Erro"] = "Agência não encontrada.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir agência {AgenciaId} por {User}", id, User.Identity?.Name);
                TempData["Erro"] = "Erro ao excluir agência. Tente novamente.";
            }

            return RedirectToAction(nameof(Index));
        }

        // ========== MÉTODOS AUXILIARES ==========

        private bool AgenciaExists(int id)
        {
            return _context.Agencias.Any(e => e.Id == id);
        }

        private async Task ValidarAgenciaUnica(Agencia agencia, int? idExcluir = null)
        {
            // Validar nome único
            var nomeExistente = await _context.Agencias
                .AnyAsync(a => a.Nome == agencia.Nome && a.Id != idExcluir);

            if (nomeExistente)
            {
                ModelState.AddModelError("Nome", "Já existe uma agência cadastrada com este nome.");
            }

            // Validar email único (se informado)
            if (!string.IsNullOrWhiteSpace(agencia.Email))
            {
                var emailExistente = await _context.Agencias
                    .AnyAsync(a => a.Email == agencia.Email && a.Id != idExcluir);

                if (emailExistente)
                {
                    ModelState.AddModelError("Email", "Já existe uma agência cadastrada com este email.");
                }
            }
        }

        private void TratarErrosBanco(DbUpdateException ex, Agencia agencia)
        {
            if (ex.InnerException != null && ex.InnerException.Message.Contains("Nome"))
            {
                ModelState.AddModelError("Nome", "Já existe uma agência com este nome.");
            }
            else if (ex.InnerException != null && ex.InnerException.Message.Contains("Email"))
            {
                ModelState.AddModelError("Email", "Já existe uma agência com este email.");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Erro ao salvar no banco de dados. Verifique os dados e tente novamente.");
            }
        }

        // ========== RELATÓRIO DA AGÊNCIA (Apenas Admin e Manager) ==========

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Relatorio(int id, DateTime? dataInicio, DateTime? dataFim)
        {
            try
            {
                var agencia = await _context.Agencias
                    .Include(a => a.Funcionarios)
                    .Include(a => a.Veiculos)
                    .Include(a => a.Locacoes)
                        .ThenInclude(l => l.Cliente)
                    .Include(a => a.Locacoes)
                        .ThenInclude(l => l.Veiculo)
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (agencia == null)
                {
                    _logger.LogWarning("Tentativa de gerar relatório de agência inexistente {AgenciaId} por {User}", id, User.Identity?.Name);
                    return NotFound();
                }

                // Definir período padrão (último mês)
                dataInicio ??= DateTime.Now.AddMonths(-1);
                dataFim ??= DateTime.Now;

                var locacoesPeriodo = agencia.Locacoes
                    .Where(l => l.DataRetirada >= dataInicio && l.DataRetirada <= dataFim)
                    .ToList();

                ViewBag.DataInicio = dataInicio;
                ViewBag.DataFim = dataFim;
                ViewBag.LocacoesPeriodo = locacoesPeriodo;
                ViewBag.ReceitaPeriodo = locacoesPeriodo.Sum(l => l.ValorTotal);
                ViewBag.LocacoesAtivas = agencia.Locacoes.Where(l => l.DataDevolucaoReal == null).Count();

                // Top funcionários do período
                ViewBag.TopFuncionarios = locacoesPeriodo
                    .GroupBy(l => new { l.FuncionarioId, l.Funcionario.Nome })
                    .Select(g => new
                    {
                        Nome = g.Key.Nome,
                        TotalLocacoes = g.Count(),
                        ReceitaTotal = g.Sum(l => l.ValorTotal)
                    })
                    .OrderByDescending(f => f.ReceitaTotal)
                    .Take(5)
                    .ToList();

                _logger.LogInformation("Relatório da agência {AgenciaId} gerado por {User} - Período: {DataInicio} a {DataFim}",
                    id, User.Identity?.Name, dataInicio?.ToString("dd/MM/yyyy"), dataFim?.ToString("dd/MM/yyyy"));

                return View(agencia);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar relatório da agência {AgenciaId} por {User}", id, User.Identity?.Name);
                TempData["Erro"] = "Erro ao gerar relatório da agência.";
                return RedirectToAction(nameof(Details), new { id = id });
            }
        }

        // ========== APIs PARA CONSUMO INTERNO (AJAX) ==========

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAgenciaData(int id)
        {
            try
            {
                var agencia = await _context.Agencias
                    .Include(a => a.Funcionarios)
                    .Include(a => a.Veiculos)
                        .ThenInclude(v => v.StatusCarro)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (agencia == null)
                {
                    return NotFound(new { message = "Agência não encontrada" });
                }

                var resultado = new
                {
                    id = agencia.Id,
                    nome = agencia.Nome,
                    endereco = agencia.Endereco,
                    telefone = agencia.Telefone,
                    email = agencia.Email,
                    totalFuncionarios = agencia.Funcionarios.Count,
                    totalVeiculos = agencia.Veiculos.Count,
                    veiculosDisponiveis = agencia.Veiculos.Count(v => v.StatusCarro.Status == "Disponível"),
                    veiculosAlugados = agencia.Veiculos.Count(v => v.StatusCarro.Status == "Alugado")
                };

                return Json(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter dados da agência {AgenciaId} via API por {User}", id, User.Identity?.Name);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        // Busca rápida de agências para autocomplete
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> BuscarAgencias(string? termo = null)
        {
            try
            {
                var query = _context.Agencias.AsQueryable();

                if (!string.IsNullOrWhiteSpace(termo))
                {
                    query = query.Where(a => a.Nome.Contains(termo) || a.Endereco.Contains(termo));
                }

                var agencias = await query
                    .Select(a => new
                    {
                        id = a.Id,
                        nome = a.Nome,
                        endereco = a.Endereco,
                        telefone = a.Telefone,
                        descricao = $"{a.Nome} - {a.Endereco}"
                    })
                    .Take(10)
                    .ToListAsync();

                return Json(agencias);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro na busca de agências por {User}", User.Identity?.Name);
                return Json(new List<object>());
            }
        }

        // Estatísticas da agência para dashboard
        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetEstatisticas(int id)
        {
            try
            {
                var agencia = await _context.Agencias
                    .Include(a => a.Funcionarios)
                    .Include(a => a.Veiculos)
                        .ThenInclude(v => v.StatusCarro)
                    .Include(a => a.Locacoes)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (agencia == null)
                {
                    return NotFound();
                }

                var hoje = DateTime.Now;
                var inicioMes = new DateTime(hoje.Year, hoje.Month, 1);

                var estatisticas = new
                {
                    totalFuncionarios = agencia.Funcionarios.Count,
                    totalVeiculos = agencia.Veiculos.Count,
                    veiculosDisponiveis = agencia.Veiculos.Count(v => v.StatusCarro.Status == "Disponível"),
                    veiculosAlugados = agencia.Veiculos.Count(v => v.StatusCarro.Status == "Alugado"),
                    locacoesMes = agencia.Locacoes.Count(l => l.DataRetirada >= inicioMes),
                    receitaMes = agencia.Locacoes.Where(l => l.DataRetirada >= inicioMes).Sum(l => l.ValorTotal),
                    locacoesAtivas = agencia.Locacoes.Count(l => l.DataDevolucaoReal == null)
                };

                return Json(estatisticas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter estatísticas da agência {AgenciaId} por {User}", id, User.Identity?.Name);
                return Json(new { error = "Erro ao carregar estatísticas" });
            }
        }
    }
}