namespace RentalTourismSystem.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<FileService> _logger;

        public FileService(IWebHostEnvironment environment, ILogger<FileService> logger)
        {
            _environment = environment;
            _logger = logger;
        }

        public async Task<(bool Success, string FilePath, string ErrorMessage)> SalvarArquivoAsync(
            IFormFile arquivo,
            string pastaDestino,
            string[]? extensoesPermitidas = null,
            long tamanhoMaximoBytes = 10485760)
        {
            try
            {
                // Validações
                if (arquivo == null || arquivo.Length == 0)
                {
                    return (false, string.Empty, "Nenhum arquivo foi enviado");
                }

                // Validar extensão
                extensoesPermitidas ??= new[] { ".pdf", ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                var extensao = Path.GetExtension(arquivo.FileName).ToLower();

                if (!extensoesPermitidas.Contains(extensao))
                {
                    return (false, string.Empty,
                        $"Tipo de arquivo não permitido. Extensões permitidas: {string.Join(", ", extensoesPermitidas)}");
                }

                // Validar tamanho
                if (arquivo.Length > tamanhoMaximoBytes)
                {
                    var tamanhoMaxMB = tamanhoMaximoBytes / 1024.0 / 1024.0;
                    return (false, string.Empty,
                        $"Arquivo muito grande. Tamanho máximo: {tamanhoMaxMB:0.##} MB");
                }

                // Criar pasta se não existir
                var pastaCompleta = Path.Combine(_environment.WebRootPath, "uploads", pastaDestino);
                if (!Directory.Exists(pastaCompleta))
                {
                    Directory.CreateDirectory(pastaCompleta);
                    _logger.LogInformation("Pasta criada: {Pasta}", pastaCompleta);
                }

                // Gerar nome único para o arquivo
                var nomeArquivoUnico = $"{Guid.NewGuid()}{extensao}";
                var caminhoCompleto = Path.Combine(pastaCompleta, nomeArquivoUnico);
                var caminhoRelativo = Path.Combine("uploads", pastaDestino, nomeArquivoUnico).Replace("\\", "/");

                // Salvar arquivo
                using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
                {
                    await arquivo.CopyToAsync(stream);
                }

                _logger.LogInformation("Arquivo salvo com sucesso: {Caminho}", caminhoCompleto);
                return (true, caminhoRelativo, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar arquivo");
                return (false, string.Empty, "Erro ao salvar arquivo no servidor");
            }
        }

        public async Task<bool> ExcluirArquivoAsync(string caminhoArquivo)
        {
            try
            {
                if (string.IsNullOrEmpty(caminhoArquivo))
                {
                    return false;
                }

                var caminhoCompleto = Path.Combine(_environment.WebRootPath, caminhoArquivo);

                if (File.Exists(caminhoCompleto))
                {
                    await Task.Run(() => File.Delete(caminhoCompleto));
                    _logger.LogInformation("Arquivo excluído: {Caminho}", caminhoCompleto);
                    return true;
                }

                _logger.LogWarning("Tentativa de excluir arquivo inexistente: {Caminho}", caminhoCompleto);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir arquivo: {Caminho}", caminhoArquivo);
                return false;
            }
        }

        public async Task<(bool Success, byte[]? FileBytes, string? ContentType, string? FileName)> ObterArquivoAsync(string caminhoArquivo)
        {
            try
            {
                if (string.IsNullOrEmpty(caminhoArquivo))
                {
                    return (false, null, null, null);
                }

                var caminhoCompleto = Path.Combine(_environment.WebRootPath, caminhoArquivo);

                if (!File.Exists(caminhoCompleto))
                {
                    _logger.LogWarning("Arquivo não encontrado: {Caminho}", caminhoCompleto);
                    return (false, null, null, null);
                }

                var bytes = await File.ReadAllBytesAsync(caminhoCompleto);
                var nomeArquivo = Path.GetFileName(caminhoCompleto);
                var contentType = ObterContentType(caminhoCompleto);

                return (true, bytes, contentType, nomeArquivo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter arquivo: {Caminho}", caminhoArquivo);
                return (false, null, null, null);
            }
        }

        public bool ValidarArquivo(IFormFile arquivo, string[] extensoesPermitidas, long tamanhoMaximoBytes)
        {
            if (arquivo == null || arquivo.Length == 0)
                return false;

            var extensao = Path.GetExtension(arquivo.FileName).ToLower();

            if (!extensoesPermitidas.Contains(extensao))
                return false;

            if (arquivo.Length > tamanhoMaximoBytes)
                return false;

            return true;
        }

        public string ObterCaminhoCompleto(string caminhoRelativo)
        {
            return Path.Combine(_environment.WebRootPath, caminhoRelativo);
        }

        private string ObterContentType(string caminho)
        {
            var extensao = Path.GetExtension(caminho).ToLower();

            return extensao switch
            {
                ".pdf" => "application/pdf",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                _ => "application/octet-stream"
            };
        }
    }
}
