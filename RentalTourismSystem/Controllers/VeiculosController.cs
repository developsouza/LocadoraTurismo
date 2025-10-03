using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RentalTourismSystem.Data;
using RentalTourismSystem.Models;

namespace RentalTourismSystem.Controllers
{
    [Authorize]
    public class VeiculosController : Controller
    {
        private readonly RentalTourismContext _context;
        private readonly ILogger<VeiculosController> _logger;

        public VeiculosController(RentalTourismContext context, ILogger<VeiculosController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Veiculos
        public async Task<IActionResult> Index(int? statusId, string? busca)
        {
            try
            {
                _logger.LogInformation("Lista de veículos acessada por usuário {User}", User.Identity?.Name);

                var veiculos = _context.Veiculos
                    .Include(v => v.StatusCarro)
                    .Include(v => v.Agencia)
                    .AsQueryable();

                if (statusId.HasValue)
                {
                    veiculos = veiculos.Where(v => v.StatusCarroId == statusId);
                    _logger.LogInformation("Filtro por status {StatusId} aplicado por {User}", statusId, User.Identity?.Name);
                }

                if (!string.IsNullOrEmpty(busca))
                {
                    veiculos = veiculos.Where(v => v.Marca.Contains(busca) ||
                                                 v.Modelo.Contains(busca) ||
                                                 v.Placa.Contains(busca));
                    _logger.LogInformation("Busca por veículos '{Busca}' realizada por {User}", busca, User.Identity?.Name);
                }

                ViewBag.StatusId = new SelectList(await _context.StatusCarros.ToListAsync(), "Id", "Status", statusId);
                ViewBag.Busca = busca;

                var listaVeiculos = await veiculos.OrderBy(v => v.Marca).ThenBy(v => v.Modelo).ToListAsync();
                return View(listaVeiculos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar lista de veículos para usuário {User}", User.Identity?.Name);
                TempData["Erro"] = "Erro ao carregar lista de veículos. Tente novamente.";
                ViewBag.StatusId = new SelectList(new List<StatusCarro>(), "Id", "Status");
                ViewBag.Busca = busca;
                return View(new List<Veiculo>());
            }
        }

        // GET: Veiculos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Tentativa de acesso a detalhes de veículo com ID nulo por {User}", User.Identity?.Name);
                return NotFound();
            }

            try
            {
                var veiculo = await _context.Veiculos
                    .Include(v => v.StatusCarro)
                    .Include(v => v.Agencia)
                    .Include(v => v.Locacoes.OrderByDescending(l => l.DataRetirada))
                        .ThenInclude(l => l.Cliente)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (veiculo == null)
                {
                    _logger.LogWarning("Veículo com ID {VeiculoId} não encontrado. Acessado por {User}", id, User.Identity?.Name);
                    return NotFound();
                }

                _logger.LogInformation("Detalhes do veículo {VeiculoId} ({Placa}) acessados por {User}",
                    id, veiculo.Placa, User.Identity?.Name);
                return View(veiculo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar detalhes do veículo {VeiculoId} para usuário {User}", id, User.Identity?.Name);
                TempData["Erro"] = "Erro ao carregar dados do veículo.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Veiculos/Create
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create()
        {
            try
            {
                await CarregarViewBags();
                _logger.LogInformation("Formulário de criação de veículo acessado por {User}", User.Identity?.Name);
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar formulário de criação de veículo por {User}", User.Identity?.Name);
                TempData["Erro"] = "Erro ao carregar formulário.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Veiculos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        // ✅ CORRIGIDO: Bind agora inclui Combustivel e Cambio
        public async Task<IActionResult> Create([Bind("Marca,Modelo,Ano,Placa,Cor,Combustivel,Cambio,ValorDiaria,Quilometragem,StatusCarroId,AgenciaId")] Veiculo veiculo)
        {
            try
            {
                // ✅ NOVO: Limpar dados antes da validação
                LimparDadosVeiculo(veiculo);

                // Validar placa única
                await ValidarPlacaUnica(veiculo);

                if (ModelState.IsValid)
                {
                    // Garantir data de cadastro
                    veiculo.DataCadastro = DateTime.Now;

                    _context.Add(veiculo);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Novo veículo {Marca} {Modelo} - {Placa} criado por {User}",
                        veiculo.Marca, veiculo.Modelo, veiculo.Placa, User.Identity?.Name);

                    TempData["Sucesso"] = "Veículo cadastrado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // Log dos erros
                    var errors = ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .Select(x => new { Field = x.Key, Errors = x.Value.Errors.Select(e => e.ErrorMessage) });

                    _logger.LogWarning("Validação falhou para veículo. Erros: {Errors}",
                        System.Text.Json.JsonSerializer.Serialize(errors));
                }
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Erro de banco ao criar veículo por {User}", User.Identity?.Name);
                TratarErrosBanco(ex, veiculo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao criar veículo por {User}", User.Identity?.Name);
                ModelState.AddModelError(string.Empty, "Erro interno do sistema. Tente novamente.");
            }

            await CarregarViewBags(veiculo.StatusCarroId, veiculo.AgenciaId);
            return View(veiculo);
        }

        // GET: Veiculos/Edit/5
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Tentativa de edição de veículo com ID nulo por {User}", User.Identity?.Name);
                return NotFound();
            }

            try
            {
                var veiculo = await _context.Veiculos
                    .Include(v => v.StatusCarro)
                    .Include(v => v.Agencia)
                    .Include(v => v.Locacoes) // ✅ CORRIGIDO: Incluir locações para estatísticas
                    .FirstOrDefaultAsync(v => v.Id == id);

                if (veiculo == null)
                {
                    _logger.LogWarning("Tentativa de edição de veículo inexistente {VeiculoId} por {User}", id, User.Identity?.Name);
                    return NotFound();
                }

                await CarregarViewBags(veiculo.StatusCarroId, veiculo.AgenciaId);
                _logger.LogInformation("Formulário de edição do veículo {VeiculoId} ({Placa}) acessado por {User}",
                    id, veiculo.Placa, User.Identity?.Name);
                return View(veiculo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar formulário de edição do veículo {VeiculoId} por {User}", id, User.Identity?.Name);
                TempData["Erro"] = "Erro ao carregar dados do veículo para edição.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Veiculos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        // ✅ CORRIGIDO: Bind agora inclui Combustivel e Cambio
        public async Task<IActionResult> Edit(int id, [Bind("Id,Marca,Modelo,Ano,Placa,Cor,Combustivel,Cambio,ValorDiaria,Quilometragem,StatusCarroId,AgenciaId")] Veiculo veiculo)
        {
            if (id != veiculo.Id)
            {
                _logger.LogWarning("Tentativa de edição com ID inconsistente {Id} != {VeiculoId} por {User}",
                    id, veiculo.Id, User.Identity?.Name);
                return NotFound();
            }

            try
            {
                // ✅ NOVO: Limpar dados antes da validação
                LimparDadosVeiculo(veiculo);

                // Validar placa única (excluindo o próprio veículo)
                await ValidarPlacaUnica(veiculo, veiculo.Id);

                if (ModelState.IsValid)
                {
                    // ✅ NOVO: Preservar a data de cadastro original
                    var veiculoOriginal = await _context.Veiculos.AsNoTracking().FirstOrDefaultAsync(v => v.Id == veiculo.Id);
                    if (veiculoOriginal != null)
                    {
                        veiculo.DataCadastro = veiculoOriginal.DataCadastro;
                    }

                    _context.Update(veiculo);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Veículo {VeiculoId} ({Placa}) atualizado por {User}",
                        veiculo.Id, veiculo.Placa, User.Identity?.Name);

                    TempData["Sucesso"] = "Veículo atualizado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // Log dos erros
                    var errors = ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .Select(x => new { Field = x.Key, Errors = x.Value.Errors.Select(e => e.ErrorMessage) });

                    _logger.LogWarning("Validação falhou para edição do veículo {VeiculoId}. Erros: {@Errors}", veiculo.Id, errors);
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!VeiculoExists(veiculo.Id))
                {
                    _logger.LogWarning("Veículo {VeiculoId} não existe mais durante edição por {User}", veiculo.Id, User.Identity?.Name);
                    return NotFound();
                }
                else
                {
                    _logger.LogError(ex, "Erro de concorrência ao editar veículo {VeiculoId} por {User}", veiculo.Id, User.Identity?.Name);
                    throw;
                }
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Erro de banco ao editar veículo {VeiculoId} por {User}", veiculo.Id, User.Identity?.Name);
                TratarErrosBanco(ex, veiculo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao editar veículo {VeiculoId} por {User}", veiculo.Id, User.Identity?.Name);
                ModelState.AddModelError(string.Empty, "Erro interno do sistema. Tente novamente.");
            }

            await CarregarViewBags(veiculo.StatusCarroId, veiculo.AgenciaId);
            return View(veiculo);
        }

        // GET: Veiculos/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Tentativa de exclusão de veículo com ID nulo por {User}", User.Identity?.Name);
                return NotFound();
            }

            try
            {
                var veiculo = await _context.Veiculos
                    .Include(v => v.StatusCarro)
                    .Include(v => v.Agencia)
                    .Include(v => v.Locacoes)
                        .ThenInclude(l => l.Cliente)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (veiculo == null)
                {
                    _logger.LogWarning("Tentativa de exclusão de veículo inexistente {VeiculoId} por {User}", id, User.Identity?.Name);
                    return NotFound();
                }

                // Verificar se há impedimentos para exclusão
                var impedimentos = new List<string>();

                var locacoesAtivas = veiculo.Locacoes.Where(l => l.DataDevolucaoReal == null).Count();
                if (locacoesAtivas > 0)
                {
                    impedimentos.Add($"{locacoesAtivas} locação(ões) ativa(s)");
                }

                var totalLocacoes = veiculo.Locacoes.Count;
                if (totalLocacoes > 0)
                {
                    impedimentos.Add($"{totalLocacoes} locação(ões) no histórico");
                }

                if (impedimentos.Any())
                {
                    ViewBag.Impedimentos = impedimentos;
                    _logger.LogInformation("Exclusão do veículo {VeiculoId} ({Placa}) bloqueada devido a impedimentos: {Impedimentos}",
                        id, veiculo.Placa, string.Join(", ", impedimentos));
                }

                _logger.LogInformation("Formulário de confirmação de exclusão do veículo {VeiculoId} ({Placa}) acessado por {User}",
                    id, veiculo.Placa, User.Identity?.Name);
                return View(veiculo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar formulário de exclusão do veículo {VeiculoId} por {User}", id, User.Identity?.Name);
                TempData["Erro"] = "Erro ao carregar dados do veículo para exclusão.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Veiculos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var veiculo = await _context.Veiculos
                    .Include(v => v.Locacoes)
                    .FirstOrDefaultAsync(v => v.Id == id);

                if (veiculo != null)
                {
                    // Verificar novamente os impedimentos
                    var locacoesAtivas = veiculo.Locacoes.Where(l => l.DataDevolucaoReal == null).Count();

                    if (locacoesAtivas > 0)
                    {
                        TempData["Erro"] = $"Não é possível excluir o veículo. Existem {locacoesAtivas} locação(ões) ativa(s) vinculada(s) a ele.";
                        _logger.LogWarning("Exclusão do veículo {VeiculoId} negada devido a locações ativas por {User}", id, User.Identity?.Name);
                        return RedirectToAction(nameof(Delete), new { id = id });
                    }

                    string infoVeiculo = $"{veiculo.Marca} {veiculo.Modelo} - {veiculo.Placa}";

                    _context.Veiculos.Remove(veiculo);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Veículo {VeiculoId} ({InfoVeiculo}) excluído por {User}",
                        id, infoVeiculo, User.Identity?.Name);

                    TempData["Sucesso"] = "Veículo excluído com sucesso!";
                }
                else
                {
                    _logger.LogWarning("Tentativa de exclusão de veículo inexistente {VeiculoId} por {User}", id, User.Identity?.Name);
                    TempData["Erro"] = "Veículo não encontrado.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir veículo {VeiculoId} por {User}", id, User.Identity?.Name);
                TempData["Erro"] = "Erro ao excluir veículo. Tente novamente.";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Veiculos/AlterarStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> AlterarStatus(int id, int novoStatusId, string? motivo)
        {
            try
            {
                var veiculo = await _context.Veiculos
                    .Include(v => v.StatusCarro)
                    .FirstOrDefaultAsync(v => v.Id == id);

                if (veiculo == null)
                {
                    _logger.LogWarning("Tentativa de alterar status de veículo inexistente {VeiculoId} por {User}", id, User.Identity?.Name);
                    return Json(new { success = false, message = "Veículo não encontrado" });
                }

                var novoStatus = await _context.StatusCarros.FindAsync(novoStatusId);
                if (novoStatus == null)
                {
                    _logger.LogWarning("Tentativa de alterar para status inexistente {StatusId} por {User}", novoStatusId, User.Identity?.Name);
                    return Json(new { success = false, message = "Status não encontrado" });
                }

                // Verificar se veículo pode ser alterado para "Disponível"
                if (novoStatus.Status == "Disponível")
                {
                    var temLocacaoAtiva = await _context.Locacoes
                        .AnyAsync(l => l.VeiculoId == id && l.DataDevolucaoReal == null);

                    if (temLocacaoAtiva)
                    {
                        return Json(new { success = false, message = "Não é possível alterar para 'Disponível'. O veículo possui locação ativa." });
                    }
                }

                var statusAnterior = veiculo.StatusCarro.Status;
                veiculo.StatusCarroId = novoStatusId;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Status do veículo {VeiculoId} ({Placa}) alterado de '{StatusAnterior}' para '{NovoStatus}' por {User}. Motivo: {Motivo}",
                    id, veiculo.Placa, statusAnterior, novoStatus.Status, User.Identity?.Name, motivo ?? "Não informado");

                return Json(new
                {
                    success = true,
                    message = $"Status alterado de '{statusAnterior}' para '{novoStatus.Status}' com sucesso!",
                    novoStatus = novoStatus.Status
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao alterar status do veículo {VeiculoId} por {User}", id, User.Identity?.Name);
                return Json(new { success = false, message = "Erro interno do sistema" });
            }
        }

        // ========== MÉTODOS AUXILIARES ==========

        private bool VeiculoExists(int id)
        {
            return _context.Veiculos.Any(e => e.Id == id);
        }

        /// <summary>
        /// ✅ NOVO: Limpa e formata dados do veículo ANTES da validação
        /// </summary>
        private void LimparDadosVeiculo(Veiculo veiculo)
        {
            // Limpar placa - remover formatação e converter para maiúsculas
            if (!string.IsNullOrEmpty(veiculo.Placa))
            {
                veiculo.Placa = veiculo.Placa
                    .Replace("-", "")
                    .Replace(" ", "")
                    .Trim()
                    .ToUpper();
            }

            // Limpar campos de texto
            if (!string.IsNullOrEmpty(veiculo.Marca))
                veiculo.Marca = veiculo.Marca.Trim();

            if (!string.IsNullOrEmpty(veiculo.Modelo))
                veiculo.Modelo = veiculo.Modelo.Trim();

            if (!string.IsNullOrEmpty(veiculo.Cor))
                veiculo.Cor = veiculo.Cor.Trim();

            if (!string.IsNullOrEmpty(veiculo.Combustivel))
                veiculo.Combustivel = veiculo.Combustivel.Trim();

            if (!string.IsNullOrEmpty(veiculo.Cambio))
                veiculo.Cambio = veiculo.Cambio.Trim();
        }

        private async Task CarregarViewBags(int? statusSelecionado = null, int? agenciaSelecionada = null)
        {
            ViewBag.StatusCarroId = new SelectList(await _context.StatusCarros.ToListAsync(), "Id", "Status", statusSelecionado);
            ViewBag.AgenciaId = new SelectList(await _context.Agencias.ToListAsync(), "Id", "Nome", agenciaSelecionada);
        }

        private async Task ValidarPlacaUnica(Veiculo veiculo, int? idExcluir = null)
        {
            var placaExistente = await _context.Veiculos
                .AnyAsync(v => v.Placa == veiculo.Placa && v.Id != idExcluir);

            if (placaExistente)
            {
                ModelState.AddModelError("Placa", "Já existe um veículo cadastrado com esta placa.");
            }
        }

        private void TratarErrosBanco(DbUpdateException ex, Veiculo veiculo)
        {
            var errorMessage = ex.InnerException?.Message ?? ex.Message;

            if (errorMessage.Contains("Placa") || errorMessage.Contains("IX_Veiculos_Placa"))
            {
                ModelState.AddModelError("Placa", "Já existe um veículo com esta placa.");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Erro ao salvar no banco de dados. Verifique os dados e tente novamente.");
                _logger.LogError("Erro de banco não tratado: {ErrorMessage}", errorMessage);
            }
        }

        // ========== APIs PARA CONSUMO INTERNO (AJAX) ==========

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetVeiculoData(int id)
        {
            try
            {
                _logger.LogInformation("API GetVeiculoData chamada para veículo {VeiculoId}", id);

                var veiculo = await _context.Veiculos
                    .Include(v => v.StatusCarro)
                    .Include(v => v.Agencia)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(v => v.Id == id);

                if (veiculo == null)
                {
                    _logger.LogWarning("Veículo {VeiculoId} não encontrado", id);
                    return NotFound(new { message = "Veículo não encontrado" });
                }

                var resultado = new
                {
                    id = veiculo.Id,
                    marca = veiculo.Marca,
                    modelo = veiculo.Modelo,
                    placa = veiculo.Placa,
                    ano = veiculo.Ano,
                    cor = veiculo.Cor,
                    combustivel = veiculo.Combustivel,
                    cambio = veiculo.Cambio,
                    valorDiaria = veiculo.ValorDiaria,
                    quilometragem = veiculo.Quilometragem,
                    status = veiculo.StatusCarro.Status,
                    statusId = veiculo.StatusCarroId,
                    agencia = veiculo.Agencia.Nome,
                    agenciaId = veiculo.AgenciaId,
                    disponivel = veiculo.StatusCarro.Status == "Disponível"
                };

                _logger.LogInformation("Dados do veículo {VeiculoId} retornados com sucesso", id);
                return Json(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter dados do veículo {VeiculoId}", id);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> VerificarDisponibilidade(int veiculoId, DateTime dataInicio, DateTime dataFim)
        {
            try
            {
                var veiculo = await _context.Veiculos
                    .Include(v => v.StatusCarro)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(v => v.Id == veiculoId);

                if (veiculo == null)
                {
                    return NotFound(new { message = "Veículo não encontrado" });
                }

                if (veiculo.StatusCarro.Status != "Disponível")
                {
                    return Json(new
                    {
                        disponivel = false,
                        motivo = $"Veículo está com status '{veiculo.StatusCarro.Status}'"
                    });
                }

                var conflito = await _context.Locacoes
                    .Where(l => l.VeiculoId == veiculoId && l.DataDevolucaoReal == null)
                    .AnyAsync(l => (dataInicio < l.DataDevolucao && dataFim > l.DataRetirada));

                if (conflito)
                {
                    return Json(new
                    {
                        disponivel = false,
                        motivo = "Veículo já está reservado para o período solicitado"
                    });
                }

                return Json(new
                {
                    disponivel = true,
                    motivo = "Veículo disponível para o período"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar disponibilidade do veículo {VeiculoId} por {User}", veiculoId, User.Identity?.Name);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> BuscarVeiculosDisponiveis(string? termo = null)
        {
            try
            {
                var query = _context.Veiculos
                    .Include(v => v.StatusCarro)
                    .Where(v => v.StatusCarro.Status == "Disponível");

                if (!string.IsNullOrWhiteSpace(termo))
                {
                    query = query.Where(v => v.Marca.Contains(termo) ||
                                           v.Modelo.Contains(termo) ||
                                           v.Placa.Contains(termo));
                }

                var veiculos = await query
                    .Select(v => new
                    {
                        id = v.Id,
                        marca = v.Marca,
                        modelo = v.Modelo,
                        placa = v.Placa,
                        ano = v.Ano,
                        valorDiaria = v.ValorDiaria,
                        descricao = $"{v.Marca} {v.Modelo} ({v.Placa}) - R$ {v.ValorDiaria:N2}/dia"
                    })
                    .Take(20)
                    .ToListAsync();

                return Json(veiculos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro na busca de veículos disponíveis por {User}", User.Identity?.Name);
                return Json(new List<object>());
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetStatusCarros()
        {
            try
            {
                var status = await _context.StatusCarros
                    .Select(s => new { id = s.Id, nome = s.Status })
                    .ToListAsync();

                return Json(status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter lista de status de carros por {User}", User.Identity?.Name);
                return Json(new List<object>());
            }
        }
    }
}