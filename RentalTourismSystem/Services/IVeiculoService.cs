using RentalTourismSystem.Models;

namespace RentalTourismSystem.Services
{
    public interface IVeiculoService
    {
        Task<ServiceResult<Veiculo>> AlterarStatusAsync(int veiculoId, int novoStatusId, string? motivo = null);
        Task<ServiceResult<Veiculo>> CriarVeiculoAsync(Veiculo veiculo);
        Task<ServiceResult<Veiculo>> EditarVeiculoAsync(Veiculo veiculo);
        Task<ServiceResult<bool>> ExcluirVeiculoAsync(int veiculoId);
        Task<bool> PodeAlterarStatusAsync(int veiculoId, int novoStatusId);
    }
}