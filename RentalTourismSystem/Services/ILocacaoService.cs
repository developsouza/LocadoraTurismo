using RentalTourismSystem.Models;

namespace RentalTourismSystem.Services
{
    public interface ILocacaoService
    {
        Task<ServiceResult<Locacao>> CriarLocacaoAsync(CriarLocacaoRequest request);
        Task<ServiceResult<Locacao>> FinalizarLocacaoAsync(int locacaoId, DateTime? dataRealDevolucao = null);
        Task<ServiceResult<Locacao>> EditarLocacaoAsync(int locacaoId, EditarLocacaoRequest request);
        Task<ServiceResult<bool>> CancelarLocacaoAsync(int locacaoId, string motivo);
        Task<decimal> CalcularValorTotalAsync(int veiculoId, DateTime dataInicio, DateTime dataFim);
        Task<bool> VerificarDisponibilidadeAsync(int veiculoId, DateTime dataInicio, DateTime dataFim, int? locacaoExcluir = null);
    }

    public class ServiceResult<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public List<string> ValidationErrors { get; set; } = new();

        public static ServiceResult<T> SuccessResult(T data) => new() { Success = true, Data = data };
        public static ServiceResult<T> ErrorResult(string error) => new() { Success = false, ErrorMessage = error };
        public static ServiceResult<T> ValidationErrorResult(List<string> errors) => new()
        {
            Success = false,
            ErrorMessage = "Dados inválidos",
            ValidationErrors = errors
        };
    }

    public class CriarLocacaoRequest
    {
        public int ClienteId { get; set; }
        public int VeiculoId { get; set; }
        public int FuncionarioId { get; set; }
        public int AgenciaId { get; set; }
        public DateTime DataRetirada { get; set; }
        public DateTime DataDevolucao { get; set; }
        public decimal ValorTotal { get; set; }
        public string? Observacoes { get; set; }
    }

    public class EditarLocacaoRequest
    {
        public DateTime DataRetirada { get; set; }
        public DateTime DataDevolucao { get; set; }
        public decimal ValorTotal { get; set; }
        public string? Observacoes { get; set; }
    }
}