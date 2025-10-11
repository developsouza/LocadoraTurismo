using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RentalTourismSystem.Data;
using RentalTourismSystem.Models;
using System.Globalization;
using System.Security.Claims;

namespace RentalTourismSystem.Controllers
{
    [Authorize] // Todo o controller requer autenticação
    public class LocacoesController : Controller
    {
        private readonly RentalTourismContext _context;
        private readonly ILogger<LocacoesController> _logger;

        public LocacoesController(RentalTourismContext context, ILogger<LocacoesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // ========== ACTIONS PRINCIPAIS (CRUD) ==========

        // GET: Locacoes - Todos os funcionários podem ver
        public async Task<IActionResult> Index(string? status, DateTime? dataInicio, DateTime? dataFim)
        {
            try
            {
                _logger.LogInformation("Lista de locações acessada por usuário {User}", User.Identity?.Name);

                var locacoes = _context.Locacoes
                    .Include(l => l.Cliente)
                    .Include(l => l.Veiculo)
                    .Include(l => l.Funcionario)
                    .Include(l => l.Agencia)
                    .AsQueryable();

                // Aplicar filtros
                if (!string.IsNullOrEmpty(status))
                {
                    switch (status.ToLower())
                    {
                        case "ativa":
                            locacoes = locacoes.Where(l => l.DataDevolucaoReal == null);
                            break;
                        case "finalizada":
                            locacoes = locacoes.Where(l => l.DataDevolucaoReal != null);
                            break;
                        case "atrasada":
                            locacoes = locacoes.Where(l => l.DataDevolucaoReal == null && l.DataDevolucao < DateTime.Now);
                            break;
                    }
                }

                if (dataInicio.HasValue)
                {
                    locacoes = locacoes.Where(l => l.DataRetirada >= dataInicio.Value.Date);
                }

                if (dataFim.HasValue)
                {
                    locacoes = locacoes.Where(l => l.DataRetirada <= dataFim.Value.Date.AddDays(1).AddTicks(-1));
                }

                ViewBag.Status = status;
                ViewBag.DataInicio = dataInicio?.ToString("yyyy-MM-dd");
                ViewBag.DataFim = dataFim?.ToString("yyyy-MM-dd");

                var listaLocacoes = await locacoes
                    .OrderByDescending(l => l.DataRetirada)
                    .ToListAsync();

                return View(listaLocacoes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar lista de locações para usuário {User}", User.Identity?.Name);
                TempData["Erro"] = "Erro ao carregar lista de locações. Tente novamente.";
                return View(new List<Locacao>());
            }
        }

        // GET: Locacoes/Details/5 - Todos podem ver detalhes
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Tentativa de acesso a detalhes de locação com ID nulo por {User}", User.Identity?.Name);
                return NotFound();
            }

            try
            {
                var locacao = await _context.Locacoes
                    .Include(l => l.Cliente)
                    .Include(l => l.Veiculo)
                        .ThenInclude(v => v.StatusCarro)
                    .Include(l => l.Funcionario)
                    .Include(l => l.Agencia)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (locacao == null)
                {
                    _logger.LogWarning("Locação com ID {LocacaoId} não encontrada. Acessado por {User}", id, User.Identity?.Name);
                    return NotFound();
                }

                _logger.LogInformation("Detalhes da locação {LocacaoId} acessados por {User}", id, User.Identity?.Name);
                return View(locacao);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar detalhes da locação {LocacaoId} para usuário {User}", id, User.Identity?.Name);
                TempData["Erro"] = "Erro ao carregar dados da locação.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Locacoes/Create - Todos os funcionários podem criar
        [Authorize(Roles = "Admin,Manager,Employee")]
        public async Task<IActionResult> Create(int? veiculoId = null, int? clienteId = null)
        {
            try
            {
                await CarregarViewBags(null, veiculoId, clienteId);

                // Passar o veiculoId para a ViewBag para uso no JavaScript
                ViewBag.VeiculoIdPreSelecionado = veiculoId;
                ViewBag.ClienteIdPreSelecionado = clienteId;

                _logger.LogInformation("Formulário de criação de locação acessado por {User} com veículo {VeiculoId} e cliente {ClienteId}",
                    User.Identity?.Name, veiculoId, clienteId);

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar formulário de criação de locação por {User}", User.Identity?.Name);
                TempData["Erro"] = "Erro ao carregar formulário.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Locacoes/Create - COM EXECUTION STRATEGY
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager,Employee")]
        public async Task<IActionResult> Create([Bind("DataRetirada,DataDevolucao,Observacoes,ClienteId,VeiculoId,FuncionarioId,AgenciaId")] Locacao locacao)
        {
            try
            {
                _logger.LogInformation("=== INICIANDO CRIAÇÃO DE LOCAÇÃO ===");
                _logger.LogInformation("Dados recebidos: ClienteId={ClienteId}, VeiculoId={VeiculoId}, DataRetirada={DataRetirada}, DataDevolucao={DataDevolucao}, FuncionarioId={FuncionarioId}, AgenciaId={AgenciaId}",
                    locacao.ClienteId, locacao.VeiculoId, locacao.DataRetirada, locacao.DataDevolucao, locacao.FuncionarioId, locacao.AgenciaId);

                // Remover validações de navegação e de ValorTotal (será calculado no servidor)
                ModelState.Remove("Agencia");
                ModelState.Remove("Cliente");
                ModelState.Remove("Veiculo");
                ModelState.Remove("Funcionario");
                ModelState.Remove(nameof(Locacao.ValorTotal));

                // Validação customizada: datas
                if (locacao.DataDevolucao <= locacao.DataRetirada)
                {
                    ModelState.AddModelError(nameof(Locacao.DataDevolucao), "Data de devolução deve ser posterior à data de retirada.");
                }

                // Recalcular ValorTotal sempre no servidor para evitar problemas de cultura
                if (locacao.VeiculoId > 0 && locacao.DataDevolucao > locacao.DataRetirada)
                {
                    locacao.ValorTotal = await CalcularValorLocacao(locacao.VeiculoId, locacao.DataRetirada, locacao.DataDevolucao);
                }

                if (!ModelState.IsValid)
                {
                    await CarregarViewBags(locacao, locacao?.VeiculoId, locacao?.ClienteId);
                    return View(locacao);
                }

                _logger.LogInformation("ModelState válido, iniciando transação com execution strategy...");

                // ✅ CORREÇÃO: Usar CreateExecutionStrategy para compatibilidade com retry
                var strategy = _context.Database.CreateExecutionStrategy();
                
                await strategy.ExecuteAsync(async () =>
                {
                    using var transaction = await _context.Database.BeginTransactionAsync();
                    try
                    {
                        _logger.LogInformation("Transação iniciada");

                        // 1) Validar disponibilidade e CNH novamente
                        var disponivel = await VerificarDisponibilidadeAsync(locacao.VeiculoId, locacao.DataRetirada, locacao.DataDevolucao);
                        if (!disponivel)
                        {
                            _logger.LogWarning("Veículo {VeiculoId} não disponível no período", locacao.VeiculoId);
                            throw new InvalidOperationException("Veículo não está mais disponível no período selecionado.");
                        }
                        _logger.LogInformation("Veículo disponível");

                        var validacaoCNH = await ValidarCNHCliente(locacao.ClienteId);
                        if (!validacaoCNH.IsValid)
                        {
                            _logger.LogWarning("CNH inválida: {ErrorMessage}", validacaoCNH.ErrorMessage);
                            throw new InvalidOperationException(validacaoCNH.ErrorMessage);
                        }
                        _logger.LogInformation("CNH válida");

                        // 2) Atualizar status do veículo para Alugado e adicionar locação
                        var statusAlugado = await _context.StatusCarros.FirstOrDefaultAsync(s => s.Status == "Alugado");
                        var veiculo = await _context.Veiculos.FirstOrDefaultAsync(v => v.Id == locacao.VeiculoId);
                        if (statusAlugado != null && veiculo != null)
                        {
                            veiculo.StatusCarroId = statusAlugado.Id;
                            _context.Veiculos.Update(veiculo);
                        }

                        _context.Locacoes.Add(locacao);

                        // 3) Persistir tudo de uma vez
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();

                        _logger.LogInformation("Locação criada com sucesso. Id={LocacaoId}", locacao.Id);
                    }
                    catch (InvalidOperationException)
                    {
                        await transaction.RollbackAsync();
                        throw; // Re-throw para capturar no catch externo
                    }
                    catch (DbUpdateException ex)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogError(ex, "DbUpdateException ao salvar locação. Mensagem: {Message}. Inner: {Inner}", ex.Message, ex.InnerException?.Message);
                        throw new InvalidOperationException($"Erro ao processar locação: {ex.InnerException?.Message ?? ex.Message}", ex);
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogError(ex, "Erro na transação ao criar locação");
                        throw new InvalidOperationException("Erro ao processar locação. Tente novamente.", ex);
                    }
                });

                TempData["Sucesso"] = "Locação criada com sucesso!";
                return RedirectToAction(nameof(Details), new { id = locacao.Id });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Erro de validação ou negócio ao criar locação");
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao criar locação por {User}", User.Identity?.Name);
                ModelState.AddModelError(string.Empty, "Erro interno do sistema. Tente novamente.");
            }

            _logger.LogInformation("Retornando view com erros");
            await CarregarViewBags(locacao, locacao?.VeiculoId, locacao?.ClienteId);
            return View(locacao);
        }

        // GET: Locacoes/Edit/5 - Apenas Admin e Manager podem editar
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Tentativa de edição de locação com ID nulo por {User}", User.Identity?.Name);
                return NotFound();
            }

            try
            {
                var locacao = await _context.Locacoes
                    .Include(l => l.Cliente)
                    .Include(l => l.Veiculo)
                    .Include(l => l.Funcionario)
                    .Include(l => l.Agencia)
                    .FirstOrDefaultAsync(l => l.Id == id);

                if (locacao == null)
                {
                    _logger.LogWarning("Tentativa de edição de locação inexistente {LocacaoId} por {User}", id, User.Identity?.Name);
                    return NotFound();
                }

                // Verificar se a locação pode ser editada
                if (locacao.DataDevolucaoReal.HasValue)
                {
                    TempData["Erro"] = "Não é possível editar uma locação já finalizada.";
                    return RedirectToAction(nameof(Details), new { id = id });
                }

                await CarregarViewBags(locacao, locacao.VeiculoId, locacao.ClienteId);
                _logger.LogInformation("Formulário de edição da locação {LocacaoId} acessado por {User}", id, User.Identity?.Name);
                return View(locacao);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar formulário de edição da locação {LocacaoId} por {User}", id, User.Identity?.Name);
                TempData["Erro"] = "Erro ao carregar dados da locação para edição.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Locacoes/Edit/5
        // POST: Locacoes/Edit/5 - COM EXECUTION STRATEGY
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DataRetirada,DataDevolucao,DataDevolucaoReal,QuilometragemRetirada,QuilometragemDevolucao,ValorTotal,Observacoes,ClienteId,VeiculoId,FuncionarioId,AgenciaId")] Locacao locacao)
        {
            if (id != locacao.Id)
            {
                _logger.LogWarning("Tentativa de edição com ID inconsistente {Id} != {LocacaoId} por {User}",
                    id, locacao.Id, User.Identity?.Name);
                return NotFound();
            }

            try
            {
                ModelState.Remove("Agencia");
                ModelState.Remove("Cliente");
                ModelState.Remove("Veiculo");
                ModelState.Remove("Funcionario");

                // Validação customizada: datas
                if (locacao.DataDevolucao <= locacao.DataRetirada)
                {
                    ModelState.AddModelError(nameof(Locacao.DataDevolucao), "Data de devolução deve ser posterior à data de retirada.");
                }

                // ✅ RECALCULAR ValorTotal sempre no servidor com base nas datas atuais
                if (locacao.VeiculoId > 0 && locacao.DataDevolucao > locacao.DataRetirada)
                {
                    var valorRecalculado = await CalcularValorLocacao(locacao.VeiculoId, locacao.DataRetirada, locacao.DataDevolucao);
                    
                    // Log para debug
                    _logger.LogInformation("Recalculando valor na edição: VeiculoId={VeiculoId}, DataRetirada={DataRetirada}, DataDevolucao={DataDevolucao}, ValorRecalculado={ValorRecalculado}, ValorInformado={ValorInformado}",
                        locacao.VeiculoId, locacao.DataRetirada, locacao.DataDevolucao, valorRecalculado, locacao.ValorTotal);
                    
                    // Atualizar o valor para o recalculado
                    locacao.ValorTotal = valorRecalculado;
                }

                if (ModelState.IsValid)
                {
                    // ✅ CORREÇÃO: Usar ExecutionStrategy
                    var strategy = _context.Database.CreateExecutionStrategy();

                    await strategy.ExecuteAsync(async () =>
                    {
                        using var transaction = await _context.Database.BeginTransactionAsync();
                        try
                        {
                            var locacaoOriginal = await _context.Locacoes.AsNoTracking().FirstOrDefaultAsync(l => l.Id == id);

                            // Se estava ativa e agora foi finalizada, liberar o veículo
                            if (!locacaoOriginal.DataDevolucaoReal.HasValue && locacao.DataDevolucaoReal.HasValue)
                            {
                                var statusDisponivel = await _context.StatusCarros
                                    .FirstOrDefaultAsync(s => s.Status == "Disponível");

                                if (statusDisponivel != null)
                                {
                                    var veiculo = await _context.Veiculos.FindAsync(locacao.VeiculoId);
                                    if (veiculo != null)
                                    {
                                        veiculo.StatusCarroId = statusDisponivel.Id;
                                        _context.Update(veiculo);
                                    }
                                }
                            }

                            _context.Update(locacao);
                            await _context.SaveChangesAsync();
                            await transaction.CommitAsync();

                            _logger.LogInformation("Locação {LocacaoId} atualizada por {User}. Novo valor total: {ValorTotal}", 
                                locacao.Id, User.Identity?.Name, locacao.ValorTotal);
                        }
                        catch
                        {
                            await transaction.RollbackAsync();
                            throw;
                        }
                    });

                    TempData["Sucesso"] = "Locação atualizada com sucesso!";
                    return RedirectToAction(nameof(Details), new { id = locacao.Id });
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!LocacaoExists(locacao.Id))
                {
                    _logger.LogWarning("Locação {LocacaoId} não existe mais durante edição por {User}", locacao.Id, User.Identity?.Name);
                    return NotFound();
                }
                else
                {
                    _logger.LogError(ex, "Erro de concorrência ao editar locação {LocacaoId} por {User}", locacao.Id, User.Identity?.Name);
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao editar locação {LocacaoId} por {User}", locacao.Id, User.Identity?.Name);
                ModelState.AddModelError(string.Empty, "Erro interno do sistema. Tente novamente.");
            }

            await CarregarViewBags(locacao, locacao.VeiculoId, locacao.ClienteId);
            return View(locacao);
        }

        // POST: Finalizar Locação - Todos os funcionários podem finalizar
        // POST: Finalizar Locação - COM EXECUTION STRATEGY
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager,Employee")]
        public async Task<IActionResult> FinalizarLocacao(int id, int? quilometragemDevolucao, string? observacoesDevolucao)
        {
            try
            {
                var locacao = await _context.Locacoes
                    .Include(l => l.Veiculo)
                    .FirstOrDefaultAsync(l => l.Id == id);

                if (locacao == null)
                {
                    _logger.LogWarning("Tentativa de finalizar locação inexistente {LocacaoId} por {User}", id, User.Identity?.Name);
                    return NotFound();
                }

                if (locacao.DataDevolucaoReal.HasValue)
                {
                    TempData["Erro"] = "Esta locação já foi finalizada.";
                    return RedirectToAction(nameof(Details), new { id = id });
                }

                // ✅ CORREÇÃO: Usar ExecutionStrategy
                var strategy = _context.Database.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    using var transaction = await _context.Database.BeginTransactionAsync();
                    try
                    {
                        // Atualizar dados da devolução
                        locacao.DataDevolucaoReal = DateTime.Now;
                        if (quilometragemDevolucao.HasValue)
                        {
                            locacao.QuilometragemDevolucao = quilometragemDevolucao.Value;
                            // Atualizar quilometragem do veículo
                            locacao.Veiculo.Quilometragem = quilometragemDevolucao.Value;
                        }

                        if (!string.IsNullOrEmpty(observacoesDevolucao))
                        {
                            locacao.Observacoes = string.IsNullOrEmpty(locacao.Observacoes)
                                ? $"Devolução: {observacoesDevolucao}"
                                : $"{locacao.Observacoes}\n\nDevolução: {observacoesDevolucao}";
                        }

                        // Calcular multa por atraso se necessário
                        if (locacao.DataDevolucaoReal > locacao.DataDevolucao)
                        {
                            var diasAtraso = (int)Math.Ceiling((locacao.DataDevolucaoReal.Value - locacao.DataDevolucao).TotalDays);
                            // Implementar lógica de multa aqui se necessário
                        }

                        // Alterar status do veículo para "Disponível"
                        var statusDisponivel = await _context.StatusCarros
                            .FirstOrDefaultAsync(s => s.Status == "Disponível");

                        if (statusDisponivel != null)
                        {
                            locacao.Veiculo.StatusCarroId = statusDisponivel.Id;
                        }

                        _context.Update(locacao);
                        _context.Update(locacao.Veiculo);
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();

                        _logger.LogInformation("Locação {LocacaoId} finalizada por {User} em {DataDevolucao}",
                            id, User.Identity?.Name, locacao.DataDevolucaoReal);

                        TempData["Sucesso"] = "Locação finalizada com sucesso!";
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogError(ex, "Erro ao finalizar locação {LocacaoId} por {User}", id, User.Identity?.Name);
                        TempData["Erro"] = "Erro ao finalizar locação. Tente novamente.";
                        throw;
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao finalizar locação {LocacaoId} por {User}", id, User.Identity?.Name);
                TempData["Erro"] = "Erro interno do sistema. Tente novamente.";
            }

            return RedirectToAction(nameof(Details), new { id = id });
        }

        // GET: Locacoes/Delete/5 - Apenas Admin pode excluir
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Tentativa de exclusão de locação com ID nulo por {User}", User.Identity?.Name);
                return NotFound();
            }

            try
            {
                var locacao = await _context.Locacoes
                    .Include(l => l.Cliente)
                    .Include(l => l.Veiculo)
                    .Include(l => l.Funcionario)
                    .Include(l => l.Agencia)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (locacao == null)
                {
                    _logger.LogWarning("Tentativa de exclusão de locação inexistente {LocacaoId} por {User}", id, User.Identity?.Name);
                    return NotFound();
                }

                // Verificar se a locação pode ser excluída
                var impedimentos = new List<string>();

                if (!locacao.DataDevolucaoReal.HasValue)
                {
                    impedimentos.Add("A locação está ativa (não foi finalizada)");
                }

                if (impedimentos.Any())
                {
                    ViewBag.Impedimentos = impedimentos;
                }

                _logger.LogInformation("Formulário de confirmação de exclusão da locação {LocacaoId} acessado por {User}", id, User.Identity?.Name);
                return View(locacao);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar formulário de exclusão da locação {LocacaoId} por {User}", id, User.Identity?.Name);
                TempData["Erro"] = "Erro ao carregar dados da locação para exclusão.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Locacoes/Delete/5
        // POST: Locacoes/Delete/5 - COM EXECUTION STRATEGY
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var locacao = await _context.Locacoes
                    .Include(l => l.Veiculo)
                    .FirstOrDefaultAsync(l => l.Id == id);

                if (locacao != null)
                {
                    // Verificar novamente se pode ser excluída
                    if (!locacao.DataDevolucaoReal.HasValue)
                    {
                        TempData["Erro"] = "Não é possível excluir uma locação ativa. Finalize a locação primeiro.";
                        return RedirectToAction(nameof(Delete), new { id = id });
                    }

                    // ✅ CORREÇÃO: Usar ExecutionStrategy
                    var strategy = _context.Database.CreateExecutionStrategy();

                    await strategy.ExecuteAsync(async () =>
                    {
                        using var transaction = await _context.Database.BeginTransactionAsync();
                        try
                        {
                            _context.Locacoes.Remove(locacao);
                            await _context.SaveChangesAsync();
                            await transaction.CommitAsync();

                            _logger.LogInformation("Locação {LocacaoId} excluída por {User}", id, User.Identity?.Name);
                            TempData["Sucesso"] = "Locação excluída com sucesso!";
                        }
                        catch (Exception ex)
                        {
                            await transaction.RollbackAsync();
                            _logger.LogError(ex, "Erro na transação ao excluir locação {LocacaoId} por {User}", id, User.Identity?.Name);
                            TempData["Erro"] = "Erro ao excluir locação. Tente novamente.";
                            throw;
                        }
                    });
                }
                else
                {
                    _logger.LogWarning("Tentativa de exclusão de locação inexistente {LocacaoId} por {User}", id, User.Identity?.Name);
                    TempData["Erro"] = "Locação não encontrada.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao excluir locação {LocacaoId} por {User}", id, User.Identity?.Name);
                TempData["Erro"] = "Erro interno do sistema. Tente novamente.";
            }

            return RedirectToAction(nameof(Index));
        }

        // ========== MÉTODOS AUXILIARES PRIVADOS ==========

        private bool LocacaoExists(int id)
        {
            return _context.Locacoes.Any(e => e.Id == id);
        }

        private async Task CarregarViewBags(Locacao? locacao = null, int? veiculoPreSelecionado = null, int? clientePreSelecionado = null)
        {
            try
            {
                // Carregar clientes com CNH válida
                var clientesList = await _context.Clientes
                    .AsNoTracking()
                    .Where(c => !string.IsNullOrEmpty(c.CNH) &&
                               c.ValidadeCNH.HasValue &&
                               c.ValidadeCNH.Value.Date >= DateTime.Now.Date)
                    .OrderBy(c => c.Nome)
                    .ToListAsync();

                ViewBag.ClienteId = new SelectList(
                    clientesList,
                    "Id", "Nome", locacao?.ClienteId ?? clientePreSelecionado);

                // Carregar veículos disponíveis ou o veículo atual/pré-selecionado
                var veiculosQuery = _context.Veiculos
                    .AsNoTracking()
                    .Include(v => v.StatusCarro)
                    .Include(v => v.Agencia)
                    .Where(v => v.StatusCarro.Status == "Disponível" ||
                               (locacao != null && v.Id == locacao.VeiculoId) ||
                               (veiculoPreSelecionado.HasValue && v.Id == veiculoPreSelecionado.Value))
                    .OrderBy(v => v.Marca)
                    .ThenBy(v => v.Modelo);

                var veiculos = await veiculosQuery.ToListAsync();
                var veiculosSelectList = veiculos.Select(v => new SelectListItem
                {
                    Value = v.Id.ToString(),
                    Text = $"{v.Marca} {v.Modelo} ({v.Placa}) - {v.ValorDiaria:C}/dia",
                    Selected = (locacao?.VeiculoId == v.Id) || (veiculoPreSelecionado == v.Id)
                }).ToList();

                ViewBag.VeiculoId = new SelectList(veiculosSelectList, "Value", "Text");

                // Carregar funcionários
                var funcionarios = await _context.Funcionarios
                    .AsNoTracking()
                    .OrderBy(f => f.Nome)
                    .ToListAsync();

                // Fallback: incluir funcionário do usuário logado se lista veio vazia
                if (funcionarios.Count == 0)
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (!string.IsNullOrEmpty(userId))
                    {
                        var usuario = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
                        if (usuario?.FuncionarioId != null)
                        {
                            var func = await _context.Funcionarios.AsNoTracking().FirstOrDefaultAsync(f => f.Id == usuario.FuncionarioId.Value);
                            if (func != null)
                            {
                                funcionarios.Add(func);
                                _logger.LogWarning("Lista de funcionários estava vazia. Adicionando funcionário do usuário logado: {FuncionarioId}", func.Id);
                            }
                        }
                    }
                }

                ViewBag.FuncionarioId = new SelectList(
                    funcionarios,
                    "Id", "Nome", locacao?.FuncionarioId);

                // Carregar agências
                var agencias = await _context.Agencias
                    .AsNoTracking()
                    .OrderBy(a => a.Nome)
                    .ToListAsync();

                if (agencias.Count == 0)
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (!string.IsNullOrEmpty(userId))
                    {
                        var usuario = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
                        if (usuario?.AgenciaId != null)
                        {
                            var ag = await _context.Agencias.AsNoTracking().FirstOrDefaultAsync(a => a.Id == usuario.AgenciaId.Value);
                            if (ag != null)
                            {
                                agencias.Add(ag);
                                _logger.LogWarning("Lista de agências estava vazia. Adicionando agência do usuário logado: {AgenciaId}", ag.Id);
                            }
                        }
                    }
                }

                ViewBag.AgenciaId = new SelectList(
                    agencias,
                    "Id", "Nome", locacao?.AgenciaId);

                // Informações adicionais para a view
                ViewBag.VeiculoIdPreSelecionado = veiculoPreSelecionado;
                ViewBag.ClienteIdPreSelecionado = clientePreSelecionado;
                ViewBag.TotalVeiculosDisponiveis = veiculos.Count();

                _logger.LogInformation("ViewBags carregadas. Clientes={Clientes}, Veiculos={Veiculos}, Funcionarios={Funcionarios}, Agencias={Agencias}",
                    clientesList.Count, veiculos.Count, funcionarios.Count, agencias.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar ViewBags");

                // Carregar listas vazias em caso de erro
                ViewBag.ClienteId = new SelectList(new List<Cliente>(), "Id", "Nome");
                ViewBag.VeiculoId = new SelectList(new List<Veiculo>(), "Id", "Marca");
                ViewBag.FuncionarioId = new SelectList(new List<Funcionario>(), "Id", "Nome");
                ViewBag.AgenciaId = new SelectList(new List<Agencia>(), "Id", "Nome");
                ViewBag.VeiculoIdPreSelecionado = null;
                ViewBag.ClienteIdPreSelecionado = null;

                throw;
            }
        }

        private async Task<bool> VerificarDisponibilidadeAsync(int veiculoId, DateTime dataInicio, DateTime dataFim, int? locacaoExcluir = null)
        {
            var ocupado = await _context.Locacoes
                .AnyAsync(l => l.VeiculoId == veiculoId &&
                         l.Id != locacaoExcluir &&
                         l.DataDevolucaoReal == null &&
                         ((dataInicio >= l.DataRetirada && dataInicio < l.DataDevolucao) ||
                          (dataFim > l.DataRetirada && dataFim <= l.DataDevolucao) ||
                          (dataInicio < l.DataRetirada && dataFim > l.DataDevolucao)));

            return !ocupado;
        }

        private async Task<ValidationResult> ValidarCNHCliente(int clienteId)
        {
            var cliente = await _context.Clientes.FindAsync(clienteId);

            if (cliente == null)
                return new ValidationResult { IsValid = false, ErrorMessage = "Cliente não encontrado." };

            if (string.IsNullOrWhiteSpace(cliente.CNH))
                return new ValidationResult { IsValid = false, ErrorMessage = "Cliente não possui número de habilitação cadastrado." };

            if (!cliente.ValidadeCNH.HasValue)
                return new ValidationResult { IsValid = false, ErrorMessage = "Cliente não possui data de validade da CNH cadastrada." };

            if (cliente.ValidadeCNH.Value.Date < DateTime.Now.Date)
                return new ValidationResult { IsValid = false, ErrorMessage = $"CNH do cliente venceu em {cliente.ValidadeCNH.Value:dd/MM/yyyy}." };

            return new ValidationResult { IsValid = true };
        }

        private async Task<decimal> CalcularValorLocacao(int veiculoId, DateTime dataRetirada, DateTime dataDevolucao)
        {
            var veiculo = await _context.Veiculos.FindAsync(veiculoId);
            if (veiculo == null) return 0;

            var totalDias = (int)Math.Ceiling((dataDevolucao - dataRetirada).TotalDays);
            if (totalDias <= 0) return 0;
            return totalDias * veiculo.ValorDiaria;
        }

        // ========== APIs PARA CONSUMO INTERNO (AJAX) ==========

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ObterDadosCliente(int id)
        {
            try
            {
                var cliente = await _context.Clientes.FindAsync(id);
                if (cliente == null)
                    return NotFound();

                return Json(new
                {
                    id = cliente.Id,
                    nome = cliente.Nome,
                    cpf = cliente.CPF,
                    email = cliente.Email,
                    telefone = cliente.Telefone,
                    numeroHabilitacao = cliente.CNH,
                    validadeCNH = cliente.ValidadeCNH?.ToString("dd/MM/yyyy"),
                    cnhValida = !string.IsNullOrWhiteSpace(cliente.CNH) &&
                               cliente.ValidadeCNH.HasValue &&
                               cliente.ValidadeCNH.Value.Date >= DateTime.Now.Date
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter dados do cliente {ClienteId} via API por {User}", id, User.Identity?.Name);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ObterDadosVeiculo(int id)
        {
            try
            {
                var veiculo = await _context.Veiculos
                    .Include(v => v.StatusCarro)
                    .FirstOrDefaultAsync(v => v.Id == id);

                if (veiculo == null)
                    return NotFound();

                return Json(new
                {
                    id = veiculo.Id,
                    marca = veiculo.Marca,
                    modelo = veiculo.Modelo,
                    placa = veiculo.Placa,
                    ano = veiculo.Ano,
                    cor = veiculo.Cor,
                    valorDiaria = veiculo.ValorDiaria,
                    quilometragem = veiculo.Quilometragem,
                    status = veiculo.StatusCarro.Status
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter dados do veículo {VeiculoId} via API por {User}", id, User.Identity?.Name);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ObterDadosVeiculoCompleto(int id)
        {
            try
            {
                var veiculo = await _context.Veiculos
                    .Include(v => v.StatusCarro)
                    .Include(v => v.Agencia)
                    .Include(v => v.Locacoes.Where(l => l.DataDevolucaoReal == null))
                        .ThenInclude(l => l.Cliente)
                    .FirstOrDefaultAsync(v => v.Id == id);

                if (veiculo == null)
                    return NotFound(new { message = "Veículo não encontrado" });

                var locacoesAtivas = veiculo.Locacoes.Where(l => l.DataDevolucaoReal == null).ToList();

                var resultado = new
                {
                    id = veiculo.Id,
                    marca = veiculo.Marca,
                    modelo = veiculo.Modelo,
                    placa = veiculo.Placa,
                    ano = veiculo.Ano,
                    cor = veiculo.Cor,
                    valorDiaria = veiculo.ValorDiaria,
                    quilometragem = veiculo.Quilometragem,
                    status = veiculo.StatusCarro.Status,
                    statusId = veiculo.StatusCarroId,
                    disponivel = veiculo.StatusCarro.Status == "Disponível" && !locacoesAtivas.Any(),
                    agencia = new
                    {
                        id = veiculo.Agencia.Id,
                        nome = veiculo.Agencia.Nome
                    },
                    locacoesAtivas = locacoesAtivas.Select(l => new
                    {
                        id = l.Id,
                        cliente = l.Cliente.Nome,
                        dataRetirada = l.DataRetirada,
                        dataDevolucao = l.DataDevolucao,
                        valorTotal = l.ValorTotal
                    }),
                    estatisticas = new
                    {
                        totalLocacoes = veiculo.Locacoes.Count,
                        valorTotalGerado = veiculo.Locacoes.Sum(l => l.ValorTotal),
                        diasNoSistema = (DateTime.Now - veiculo.DataCadastro).Days
                    }
                };

                return Json(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter dados completos do veículo {VeiculoId}", id);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> CalcularValor(int veiculoId, DateTime dataRetirada, DateTime dataDevolucao)
        {
            try
            {
                _logger.LogInformation("=== CALCULANDO VALOR ===");
                _logger.LogInformation("VeiculoId: {VeiculoId}, DataRetirada: {DataRetirada}, DataDevolucao: {DataDevolucao}",
                    veiculoId, dataRetirada, dataDevolucao);

                var veiculo = await _context.Veiculos.FindAsync(veiculoId);
                if (veiculo == null) return Json(new { valor = 0, totalDias = 0 });

                var totalDias = (int)Math.Ceiling((dataDevolucao - dataRetirada).TotalDays);
                var valorCalculado = totalDias > 0 ? totalDias * veiculo.ValorDiaria : 0;

                _logger.LogInformation("Diária: {ValorDiaria}, Dias: {TotalDias}, Total: {ValorCalculado}",
                    veiculo.ValorDiaria, totalDias, valorCalculado);

                return Json(new
                {
                    valor = valorCalculado,
                    totalDias = totalDias
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao calcular valor");
                return Json(new { valor = 0, totalDias = 0 });
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ValidarCNHClienteAPI(int clienteId)
        {
            try
            {
                var resultado = await ValidarCNHCliente(clienteId);

                return Json(new
                {
                    valida = resultado.IsValid,
                    mensagem = resultado.ErrorMessage ?? "CNH válida para locação"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar CNH do cliente {ClienteId} via API por {User}", clienteId, User.Identity?.Name);
                return Json(new
                {
                    valida = false,
                    mensagem = "Erro ao validar CNH"
                });
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ValidarDisponibilidadeVeiculo(int veiculoId, DateTime dataInicio, DateTime dataFim, int? locacaoExcluir = null)
        {
            try
            {
                _logger.LogInformation("Validando disponibilidade do veículo {VeiculoId} de {DataInicio} a {DataFim}",
                    veiculoId, dataInicio, dataFim);

                var veiculo = await _context.Veiculos
                    .Include(v => v.StatusCarro)
                    .FirstOrDefaultAsync(v => v.Id == veiculoId);

                if (veiculo == null)
                {
                    return Json(new
                    {
                        disponivel = false,
                        motivo = "Veículo não encontrado",
                        codigo = "VEICULO_NAO_ENCONTRADO"
                    });
                }

                var statusPermitidos = new[] { "Disponível", "Alugado" };
                if (!statusPermitidos.Contains(veiculo.StatusCarro.Status))
                {
                    return Json(new
                    {
                        disponivel = false,
                        motivo = $"Veículo está em status: {veiculo.StatusCarro.Status}",
                        codigo = "STATUS_INVALIDO",
                        status = veiculo.StatusCarro.Status
                    });
                }

                var disponivel = await VerificarDisponibilidadeAsync(veiculoId, dataInicio, dataFim, locacaoExcluir);

                if (!disponivel)
                {
                    var locacoesConflitantes = await _context.Locacoes
                        .Include(l => l.Cliente)
                        .Where(l => l.VeiculoId == veiculoId &&
                                   l.Id != locacaoExcluir &&
                                   l.DataDevolucaoReal == null &&
                                   ((l.DataRetirada < dataFim && l.DataDevolucao > dataInicio)))
                        .ToListAsync();

                    return Json(new
                    {
                        disponivel = false,
                        motivo = "Veículo já está alugado no período solicitado",
                        codigo = "PERIODO_OCUPADO",
                        conflitos = locacoesConflitantes.Select(l => new
                        {
                            id = l.Id,
                            cliente = l.Cliente.Nome,
                            dataRetirada = l.DataRetirada.ToString("dd/MM/yyyy HH:mm"),
                            dataDevolucao = l.DataDevolucao.ToString("dd/MM/yyyy HH:mm")
                        })
                    });
                }

                return Json(new
                {
                    disponivel = true,
                    motivo = "Veículo disponível",
                    codigo = "DISPONIVEL",
                    dadosVeiculo = new
                    {
                        id = veiculo.Id,
                        marca = veiculo.Marca,
                        modelo = veiculo.Modelo,
                        placa = veiculo.Placa,
                        valorDiaria = veiculo.ValorDiaria,
                        status = veiculo.StatusCarro.Status
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar disponibilidade do veículo {VeiculoId}", veiculoId);
                return Json(new
                {
                    disponivel = false,
                    motivo = "Erro interno do sistema",
                    codigo = "ERRO_INTERNO"
                });
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> VerificarVeiculosDisponiveis([FromBody] VerificarDisponibilidadeRequest request)
        {
            try
            {
                if (request.DataInicio >= request.DataFim)
                {
                    return BadRequest(new { message = "Data de fim deve ser posterior à data de início" });
                }

                var query = _context.Veiculos
                    .Include(v => v.StatusCarro)
                    .Include(v => v.Agencia)
                    .AsQueryable();

                if (request.AgenciaId.HasValue)
                {
                    query = query.Where(v => v.AgenciaId == request.AgenciaId.Value);
                }

                query = query.Where(v => v.StatusCarro.Status == "Disponível");

                var veiculos = await query.ToListAsync();
                var veiculosDisponiveis = new List<object>();

                foreach (var veiculo in veiculos)
                {
                    var disponivel = await VerificarDisponibilidadeAsync(veiculo.Id, request.DataInicio, request.DataFim);

                    if (disponivel)
                    {
                        veiculosDisponiveis.Add(new
                        {
                            id = veiculo.Id,
                            marca = veiculo.Marca,
                            modelo = veiculo.Modelo,
                            placa = veiculo.Placa,
                            ano = veiculo.Ano,
                            cor = veiculo.Cor,
                            valorDiaria = veiculo.ValorDiaria,
                            agencia = veiculo.Agencia.Nome,
                            valorTotal = await CalcularValorLocacao(veiculo.Id, request.DataInicio, request.DataFim)
                        });
                    }
                }

                return Json(new
                {
                    periodo = new
                    {
                        inicio = request.DataInicio.ToString("dd/MM/yyyy HH:mm"),
                        fim = request.DataFim.ToString("dd/MM/yyyy HH:mm"),
                        dias = Math.Ceiling((request.DataFim - request.DataInicio).TotalDays)
                    },
                    totalDisponiveis = veiculosDisponiveis.Count,
                    veiculos = veiculosDisponiveis
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar veículos disponíveis");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }
    }

    // ========== CLASSES AUXILIARES ==========

    public class VerificarDisponibilidadeRequest
    {
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public int? AgenciaId { get; set; }
    }

    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }
}