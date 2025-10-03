using Microsoft.EntityFrameworkCore;
using RentalTourismSystem.Data;
using RentalTourismSystem.Models;

namespace RentalTourismSystem.Services
{
    public class LocacaoService : ILocacaoService
    {
        private readonly RentalTourismContext _context;
        private readonly ILogger<LocacaoService> _logger;

        public LocacaoService(RentalTourismContext context, ILogger<LocacaoService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ServiceResult<Locacao>> CriarLocacaoAsync(CriarLocacaoRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _logger.LogInformation("Iniciando criação de locação - Cliente: {ClienteId}, Veículo: {VeiculoId}",
                    request.ClienteId, request.VeiculoId);

                // 1. Validações de negócio
                var validationResult = await ValidarCriacaoLocacaoAsync(request);
                if (!validationResult.Success)
                {
                    return validationResult;
                }

                // 2. Verificar disponibilidade final
                var disponivel = await VerificarDisponibilidadeAsync(request.VeiculoId, request.DataRetirada, request.DataDevolucao);
                if (!disponivel)
                {
                    return ServiceResult<Locacao>.ErrorResult("Veículo não está disponível no período selecionado");
                }

                // 3. Criar a locação
                var locacao = new Locacao
                {
                    ClienteId = request.ClienteId,
                    VeiculoId = request.VeiculoId,
                    FuncionarioId = request.FuncionarioId,
                    AgenciaId = request.AgenciaId,
                    DataRetirada = request.DataRetirada,
                    DataDevolucao = request.DataDevolucao,
                    ValorTotal = request.ValorTotal,
                    Observacoes = request.Observacoes
                };

                _context.Locacoes.Add(locacao);
                await _context.SaveChangesAsync();

                // 4. Atualizar status do veículo para "Alugado"
                var veiculo = await _context.Veiculos.FindAsync(request.VeiculoId);
                if (veiculo != null)
                {
                    veiculo.StatusCarroId = 2; // Alugado
                    _context.Update(veiculo);
                    await _context.SaveChangesAsync();
                }

                // 5. Commit da transação
                await transaction.CommitAsync();

                // 6. Recarregar com dados relacionados para retorno
                await _context.Entry(locacao)
                    .Reference(l => l.Cliente)
                    .LoadAsync();
                await _context.Entry(locacao)
                    .Reference(l => l.Veiculo)
                    .LoadAsync();

                _logger.LogInformation("Locação {LocacaoId} criada com sucesso para cliente {ClienteId} e veículo {VeiculoId}",
                    locacao.Id, request.ClienteId, request.VeiculoId);

                return ServiceResult<Locacao>.SuccessResult(locacao);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Erro ao criar locação - Cliente: {ClienteId}, Veículo: {VeiculoId}",
                    request.ClienteId, request.VeiculoId);

                return ServiceResult<Locacao>.ErrorResult($"Erro interno: {ex.Message}");
            }
        }

        public async Task<ServiceResult<Locacao>> FinalizarLocacaoAsync(int locacaoId, DateTime? dataRealDevolucao = null)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _logger.LogInformation("Finalizando locação {LocacaoId}", locacaoId);

                var locacao = await _context.Locacoes
                    .Include(l => l.Veiculo)
                    .Include(l => l.Cliente)
                    .FirstOrDefaultAsync(l => l.Id == locacaoId);

                if (locacao == null)
                {
                    return ServiceResult<Locacao>.ErrorResult("Locação não encontrada");
                }

                if (locacao.DataDevolucaoReal.HasValue)
                {
                    return ServiceResult<Locacao>.ErrorResult("Locação já foi finalizada");
                }

                // Definir data real de devolução
                locacao.DataDevolucaoReal = dataRealDevolucao ?? DateTime.Now;

                // Calcular multa se em atraso
                if (locacao.DataDevolucaoReal > locacao.DataDevolucao)
                {
                    var diasAtraso = (int)(locacao.DataDevolucaoReal.Value.Date - locacao.DataDevolucao.Date).TotalDays;
                    var multa = locacao.ValorTotal * 0.1m * diasAtraso; // 10% por dia

                    locacao.Observacoes += $"\nMULTA: {diasAtraso} dia(s) de atraso - R$ {multa:F2}";

                    _logger.LogWarning("Locação {LocacaoId} finalizada com atraso: {DiasAtraso} dias, multa: R$ {Multa}",
                        locacaoId, diasAtraso, multa);
                }

                _context.Update(locacao);

                // Liberar veículo (status = Disponível)
                if (locacao.Veiculo != null)
                {
                    locacao.Veiculo.StatusCarroId = 1; // Disponível
                    _context.Update(locacao.Veiculo);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Locação {LocacaoId} finalizada com sucesso - Cliente: {ClienteNome}, Veículo: {VeiculoPlaca}",
                    locacaoId, locacao.Cliente.Nome, locacao.Veiculo.Placa);

                return ServiceResult<Locacao>.SuccessResult(locacao);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Erro ao finalizar locação {LocacaoId}", locacaoId);

                return ServiceResult<Locacao>.ErrorResult($"Erro interno: {ex.Message}");
            }
        }

        public async Task<ServiceResult<Locacao>> EditarLocacaoAsync(int locacaoId, EditarLocacaoRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _logger.LogInformation("Editando locação {LocacaoId}", locacaoId);

                var locacao = await _context.Locacoes
                    .Include(l => l.Veiculo)
                    .FirstOrDefaultAsync(l => l.Id == locacaoId);

                if (locacao == null)
                {
                    return ServiceResult<Locacao>.ErrorResult("Locação não encontrada");
                }

                if (locacao.DataDevolucaoReal.HasValue)
                {
                    return ServiceResult<Locacao>.ErrorResult("Não é possível editar locação já finalizada");
                }

                // Verificar disponibilidade se houve mudança nas datas
                if (locacao.DataRetirada != request.DataRetirada || locacao.DataDevolucao != request.DataDevolucao)
                {
                    var disponivel = await VerificarDisponibilidadeAsync(
                        locacao.VeiculoId,
                        request.DataRetirada,
                        request.DataDevolucao,
                        locacaoId);

                    if (!disponivel)
                    {
                        return ServiceResult<Locacao>.ErrorResult("Veículo não está disponível no novo período selecionado");
                    }
                }

                // Atualizar dados
                locacao.DataRetirada = request.DataRetirada;
                locacao.DataDevolucao = request.DataDevolucao;
                locacao.ValorTotal = request.ValorTotal;
                locacao.Observacoes = request.Observacoes;

                _context.Update(locacao);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Locação {LocacaoId} editada com sucesso", locacaoId);

                return ServiceResult<Locacao>.SuccessResult(locacao);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Erro ao editar locação {LocacaoId}", locacaoId);

                return ServiceResult<Locacao>.ErrorResult($"Erro interno: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> CancelarLocacaoAsync(int locacaoId, string motivo)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _logger.LogInformation("Cancelando locação {LocacaoId}, motivo: {Motivo}", locacaoId, motivo);

                var locacao = await _context.Locacoes
                    .Include(l => l.Veiculo)
                    .FirstOrDefaultAsync(l => l.Id == locacaoId);

                if (locacao == null)
                {
                    return ServiceResult<bool>.ErrorResult("Locação não encontrada");
                }

                if (locacao.DataDevolucaoReal.HasValue)
                {
                    return ServiceResult<bool>.ErrorResult("Não é possível cancelar locação já finalizada");
                }

                // Marcar como cancelada
                locacao.DataDevolucaoReal = DateTime.Now;
                locacao.Observacoes += $"\n*** CANCELADA *** - Motivo: {motivo} - Data: {DateTime.Now:dd/MM/yyyy HH:mm}";

                _context.Update(locacao);

                // Liberar veículo
                if (locacao.Veiculo != null)
                {
                    locacao.Veiculo.StatusCarroId = 1; // Disponível
                    _context.Update(locacao.Veiculo);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Locação {LocacaoId} cancelada com sucesso", locacaoId);

                return ServiceResult<bool>.SuccessResult(true);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Erro ao cancelar locação {LocacaoId}", locacaoId);

                return ServiceResult<bool>.ErrorResult($"Erro interno: {ex.Message}");
            }
        }

        public async Task<decimal> CalcularValorTotalAsync(int veiculoId, DateTime dataInicio, DateTime dataFim)
        {
            try
            {
                var veiculo = await _context.Veiculos.FindAsync(veiculoId);
                if (veiculo == null) return 0;

                var dias = (int)(dataFim.Date - dataInicio.Date).TotalDays;
                if (dias <= 0) return 0;

                return veiculo.ValorDiaria * dias;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao calcular valor total para veículo {VeiculoId}", veiculoId);
                return 0;
            }
        }

        public async Task<bool> VerificarDisponibilidadeAsync(int veiculoId, DateTime dataInicio, DateTime dataFim, int? locacaoExcluir = null)
        {
            try
            {
                // Verificar status do veículo
                var veiculo = await _context.Veiculos.FindAsync(veiculoId);
                if (veiculo == null || (veiculo.StatusCarroId != 1 && veiculo.StatusCarroId != 2)) // Disponível ou Alugado
                {
                    return false;
                }

                // Verificar conflitos com outras locações ativas
                var conflitos = await _context.Locacoes
                    .Where(l => l.VeiculoId == veiculoId &&
                               l.Id != locacaoExcluir && // Excluir locação específica se informada
                               l.DataDevolucaoReal == null && // Apenas locações ativas
                               ((l.DataRetirada < dataFim && l.DataDevolucao > dataInicio))) // Sobreposição de período
                    .CountAsync();

                return conflitos == 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar disponibilidade do veículo {VeiculoId}", veiculoId);
                return false;
            }
        }

        private async Task<ServiceResult<Locacao>> ValidarCriacaoLocacaoAsync(CriarLocacaoRequest request)
        {
            var erros = new List<string>();

            // Validar cliente
            var cliente = await _context.Clientes.FindAsync(request.ClienteId);
            if (cliente == null)
            {
                erros.Add("Cliente não encontrado");
            }
            else
            {
                // Validar idade
                var idade = DateTime.Now.Year - cliente.DataNascimento.Year;
                if (DateTime.Now.DayOfYear < cliente.DataNascimento.DayOfYear) idade--;

                if (idade < 21)
                {
                    erros.Add("Cliente deve ter pelo menos 21 anos para locação");
                }

                // Validar CNH
                if (string.IsNullOrEmpty(cliente.CNH))
                {
                    erros.Add("Cliente deve ter CNH cadastrada");
                }
                else if (!cliente.ValidadeCNH.HasValue)
                {
                    erros.Add("Data de validade da CNH é obrigatória");
                }
                else if (cliente.ValidadeCNH.Value.Date < DateTime.Now.Date)
                {
                    erros.Add("CNH do cliente está vencida");
                }
            }

            // Validar veículo
            var veiculo = await _context.Veiculos
                .Include(v => v.StatusCarro)
                .FirstOrDefaultAsync(v => v.Id == request.VeiculoId);

            if (veiculo == null)
            {
                erros.Add("Veículo não encontrado");
            }
            else if (veiculo.StatusCarroId != 1) // Não está disponível
            {
                erros.Add($"Veículo está com status: {veiculo.StatusCarro.Status}");
            }

            // Validar datas
            if (request.DataRetirada < DateTime.Now.Date)
            {
                erros.Add("Data de retirada não pode ser no passado");
            }

            if (request.DataDevolucao <= request.DataRetirada)
            {
                erros.Add("Data de devolução deve ser posterior à data de retirada");
            }

            var diferencaHoras = (request.DataDevolucao - request.DataRetirada).TotalHours;
            if (diferencaHoras < 24)
            {
                erros.Add("Período mínimo de locação é de 24 horas");
            }

            // Validar funcionário e agência
            if (!await _context.Funcionarios.AnyAsync(f => f.Id == request.FuncionarioId))
            {
                erros.Add("Funcionário não encontrado");
            }

            if (!await _context.Agencias.AnyAsync(a => a.Id == request.AgenciaId))
            {
                erros.Add("Agência não encontrada");
            }

            if (erros.Any())
            {
                return ServiceResult<Locacao>.ValidationErrorResult(erros);
            }

            return ServiceResult<Locacao>.SuccessResult(new Locacao());
        }
    }
}