using RentalTourismSystem.Models;

namespace RentalTourismSystem.Services
{
    public interface IFileService
    {
        /// <summary>
        /// Salva um arquivo no sistema de arquivos
        /// </summary>
        Task<(bool Success, string FilePath, string ErrorMessage)> SalvarArquivoAsync(
            IFormFile arquivo, 
            string pastaDestino, 
            string[]? extensoesPermitidas = null, 
            long tamanhoMaximoBytes = 10485760); // 10MB padrão

        /// <summary>
        /// Exclui um arquivo do sistema
        /// </summary>
        Task<bool> ExcluirArquivoAsync(string caminhoArquivo);

        /// <summary>
        /// Obtém um arquivo para download
        /// </summary>
        Task<(bool Success, byte[]? FileBytes, string? ContentType, string? FileName)> ObterArquivoAsync(string caminhoArquivo);

        /// <summary>
        /// Valida se o arquivo é permitido
        /// </summary>
        bool ValidarArquivo(IFormFile arquivo, string[] extensoesPermitidas, long tamanhoMaximoBytes);

        /// <summary>
        /// Obtém o caminho completo do arquivo
        /// </summary>
        string ObterCaminhoCompleto(string caminhoRelativo);
    }
}
