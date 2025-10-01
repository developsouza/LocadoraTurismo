using Microsoft.EntityFrameworkCore;
using RentalTourismSystem.Data;
using RentalTourismSystem.Models;

namespace RentalTourismSystem.Services
{
    public class VeiculoService : IVeiculoService
    {
        private readonly RentalTourismContext _context;
        private readonly ILogger<VeiculoService> _logger;

        public VeiculoService(RentalTourismContext context, ILogger<VeiculoService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ServiceResult<Veiculo>> AlterarStatusAsync(int veiculoId, int novoStatusId, string? motivo = null)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _logger.LogInformation("Alterando status do veículo {VeiculoId} para status {StatusId}", veiculoId, novoStatusId);

                var veiculo = await _context.Veiculos
                    .Include(v => v.StatusCarro)
                    .FirstOrDefaultAsync(v => v.Id == veiculoId);

                if (veiculo == null)
                {
                    return ServiceResult<Veiculo>.ErrorResult("Veículo não encontrado");
                }

                var novoStatus = await _context.StatusCarros.FindAsync(novoStatusId);
                if (novoStatus == null)
                {
                    return ServiceResult<Veiculo>.ErrorResult("Status inválido");
                }

                // Validar mudança de status
                var podeAlterar = await PodeAlterarStatusAsync(veiculoId, novoStatusId);
                if (!podeAlterar)
                {
                    var locacoesAtivas = await _context.Locacoes.CountAsync(l => l.VeiculoId == veiculoId && l.DataDevolucaoReal == null);
                    if (locacoesAtivas > 0)
                    {
                        return ServiceResult<Veiculo>.ErrorResult("Não é possível alterar status: veículo possui locações ativas");
                    }
                }

                var statusAnterior = veiculo.StatusCarro.Status;
                veiculo.StatusCarroId = novoStatusId;

                // Log de auditoria
                var logEntry = new
                {
                    VeiculoId = veiculoId,
                    Placa = veiculo.Placa,
                    StatusAnterior = statusAnterior,
                    NovoStatus = novoStatus.Status,
                    Motivo = motivo,
                    Data = DateTime.Now
                };

                _logger.LogInformation("Status alterado: {LogEntry}", logEntry);

                _context.Update(veiculo);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Recarregar com status atualizado
                await _context.Entry(veiculo).Reference(v => v.StatusCarro).LoadAsync();

                return ServiceResult<Veiculo>.SuccessResult(veiculo);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Erro ao alterar status do veículo {VeiculoId}", veiculoId);

                return ServiceResult<Veiculo>.ErrorResult($"Erro interno: {ex.Message}");
            }
        }

        public async Task<ServiceResult<Veiculo>> CriarVeiculoAsync(Veiculo veiculo)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _logger.LogInformation("Criando novo veículo - Placa: {Placa}", veiculo.Placa);

                // Verificar duplicação de placa
                var placaExistente = await _context.Veiculos.AnyAsync(v => v.Placa == veiculo.Placa);
                if (placaExistente)
                {
                    return ServiceResult<Veiculo>.ErrorResult("Já existe um veículo cadastrado com esta placa");
                }

                // Definir status padrão como "Disponível" se não especificado
                if (veiculo.StatusCarroId == 0)
                {
                    veiculo.StatusCarroId = 1; // Disponível
                }

                _context.Veiculos.Add(veiculo);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Veículo criado com sucesso - ID: {VeiculoId}, Placa: {Placa}", veiculo.Id, veiculo.Placa);

                return ServiceResult<Veiculo>.SuccessResult(veiculo);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Erro ao criar veículo - Placa: {Placa}", veiculo.Placa);

                return ServiceResult<Veiculo>.ErrorResult($"Erro interno: {ex.Message}");
            }
        }

        public async Task<ServiceResult<Veiculo>> EditarVeiculoAsync(Veiculo veiculo)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _logger.LogInformation("Editando veículo {VeiculoId}", veiculo.Id);

                var veiculoExistente = await _context.Veiculos.FindAsync(veiculo.Id);
                if (veiculoExistente == null)
                {
                    return ServiceResult<Veiculo>.ErrorResult("Veículo não encontrado");
                }

                // Verificar duplicação de placa (exceto o próprio veículo)
                var placaExistente = await _context.Veiculos
                    .AnyAsync(v => v.Placa == veiculo.Placa && v.Id != veiculo.Id);

                if (placaExistente)
                {
                    return ServiceResult<Veiculo>.ErrorResult("Já existe outro veículo cadastrado com esta placa");
                }

                // Atualizar dados
                veiculoExistente.Marca = veiculo.Marca;
                veiculoExistente.Modelo = veiculo.Modelo;
                veiculoExistente.Ano = veiculo.Ano;
                veiculoExistente.Placa = veiculo.Placa;
                veiculoExistente.Cor = veiculo.Cor;
                veiculoExistente.ValorDiaria = veiculo.ValorDiaria;
                veiculoExistente.Quilometragem = veiculo.Quilometragem;
                veiculoExistente.StatusCarroId = veiculo.StatusCarroId;
                veiculoExistente.AgenciaId = veiculo.AgenciaId;

                _context.Update(veiculoExistente);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Veículo {VeiculoId} editado com sucesso", veiculo.Id);

                return ServiceResult<Veiculo>.SuccessResult(veiculoExistente);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Erro ao editar veículo {VeiculoId}", veiculo.Id);

                return ServiceResult<Veiculo>.ErrorResult($"Erro interno: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> ExcluirVeiculoAsync(int veiculoId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _logger.LogInformation("Excluindo veículo {VeiculoId}", veiculoId);

                var veiculo = await _context.Veiculos
                    .Include(v => v.Locacoes)
                    .FirstOrDefaultAsync(v => v.Id == veiculoId);

                if (veiculo == null)
                {
                    return ServiceResult<bool>.ErrorResult("Veículo não encontrado");
                }

                // Verificar se há locações vinculadas
                if (veiculo.Locacoes.Any())
                {
                    var locacoesAtivas = veiculo.Locacoes.Count(l => l.DataDevolucaoReal == null);
                    var locacoesFinalizadas = veiculo.Locacoes.Count(l => l.DataDevolucaoReal != null);

                    return ServiceResult<bool>.ErrorResult(
                        $"Não é possível excluir: veículo possui {veiculo.Locacoes.Count} locação(ões) - " +
                        $"{locacoesAtivas} ativa(s) e {locacoesFinalizadas} finalizada(s)");
                }

                _context.Veiculos.Remove(veiculo);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Veículo {VeiculoId} excluído com sucesso", veiculoId);

                return ServiceResult<bool>.SuccessResult(true);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Erro ao excluir veículo {VeiculoId}", veiculoId);

                return ServiceResult<bool>.ErrorResult($"Erro interno: {ex.Message}");
            }
        }

        public async Task<bool> PodeAlterarStatusAsync(int veiculoId, int novoStatusId)
        {
            try
            {
                // Status "Disponível" - só pode se não houver locações ativas
                if (novoStatusId == 1)
                {
                    var locacoesAtivas = await _context.Locacoes
                        .CountAsync(l => l.VeiculoId == veiculoId && l.DataDevolucaoReal == null);

                    return locacoesAtivas == 0;
                }

                // Outros status sempre podem ser alterados
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar se pode alterar status do veículo {VeiculoId}", veiculoId);
                return false;
            }
        }
    }
}