using Microsoft.AspNetCore.StaticFiles;

namespace RentalTourismSystem.Services;

public class FileService : IFileService
{
    private readonly string _storageRoot;
    private readonly ILogger<FileService> _logger;
    private static readonly FileExtensionContentTypeProvider ContentTypes = new();

    public FileService(IWebHostEnvironment environment, ILogger<FileService> logger)
    {
        _storageRoot = Path.GetFullPath(Path.Combine(environment.ContentRootPath, "uploads"));
        _logger = logger;
    }

    public async Task<(bool Success, string FilePath, string ErrorMessage)> SalvarArquivoAsync(
        IFormFile arquivo, string pastaDestino, string[]? extensoesPermitidas = null,
        long tamanhoMaximoBytes = 10 * 1024 * 1024)
    {
        if (arquivo is null || arquivo.Length <= 0)
            return (false, string.Empty, "Nenhum arquivo foi enviado");
        if (arquivo.Length > tamanhoMaximoBytes)
            return (false, string.Empty, $"Arquivo muito grande. Tamanho máximo: {tamanhoMaximoBytes / 1024d / 1024d:0.##} MB");

        extensoesPermitidas ??= [".pdf", ".jpg", ".jpeg", ".png"];
        var extensao = Path.GetExtension(arquivo.FileName).ToLowerInvariant();
        if (!extensoesPermitidas.Contains(extensao, StringComparer.OrdinalIgnoreCase))
            return (false, string.Empty, "Tipo de arquivo não permitido");

        try
        {
            await using var input = arquivo.OpenReadStream();
            if (!await AssinaturaValidaAsync(input, extensao))
                return (false, string.Empty, "O conteúdo do arquivo não corresponde ao tipo informado");

            var pastaCompleta = ResolveSafePath(pastaDestino);
            Directory.CreateDirectory(pastaCompleta);
            var nome = $"{Guid.NewGuid():N}{extensao}";
            var destino = Path.Combine(pastaCompleta, nome);
            input.Position = 0;
            await using var output = new FileStream(destino, FileMode.CreateNew, FileAccess.Write, FileShare.None, 81920, FileOptions.Asynchronous);
            await input.CopyToAsync(output);

            var relativo = Path.GetRelativePath(_storageRoot, destino).Replace('\\', '/');
            _logger.LogInformation("Arquivo armazenado com segurança em {CaminhoRelativo}", relativo);
            return (true, relativo, string.Empty);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao armazenar arquivo");
            return (false, string.Empty, "Erro ao salvar arquivo no servidor");
        }
    }

    public Task<bool> ExcluirArquivoAsync(string caminhoArquivo)
    {
        try
        {
            var caminho = ResolveSafePath(caminhoArquivo);
            if (!File.Exists(caminho)) return Task.FromResult(false);
            File.Delete(caminho);
            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Caminho de arquivo inválido ou erro ao excluir");
            return Task.FromResult(false);
        }
    }

    public async Task<(bool Success, byte[]? FileBytes, string? ContentType, string? FileName)> ObterArquivoAsync(string caminhoArquivo)
    {
        try
        {
            var caminho = ResolveSafePath(caminhoArquivo);
            if (!File.Exists(caminho)) return (false, null, null, null);
            var tipo = ContentTypes.TryGetContentType(caminho, out var encontrado) ? encontrado : "application/octet-stream";
            return (true, await File.ReadAllBytesAsync(caminho), tipo, Path.GetFileName(caminho));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Caminho de arquivo inválido ou erro ao ler");
            return (false, null, null, null);
        }
    }

    public bool ValidarArquivo(IFormFile arquivo, string[] extensoesPermitidas, long tamanhoMaximoBytes) =>
        arquivo is { Length: > 0 } && arquivo.Length <= tamanhoMaximoBytes &&
        extensoesPermitidas.Contains(Path.GetExtension(arquivo.FileName), StringComparer.OrdinalIgnoreCase);

    public string ObterCaminhoCompleto(string caminhoRelativo) => ResolveSafePath(caminhoRelativo);

    private string ResolveSafePath(string caminhoRelativo)
    {
        if (string.IsNullOrWhiteSpace(caminhoRelativo)) throw new ArgumentException("Caminho vazio");
        var normalizado = caminhoRelativo.Replace('\\', '/').TrimStart('/');
        if (normalizado.StartsWith("uploads/", StringComparison.OrdinalIgnoreCase)) normalizado = normalizado[8..];
        var completo = Path.GetFullPath(Path.Combine(_storageRoot, normalizado));
        var prefixo = _storageRoot.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
        if (!completo.StartsWith(prefixo, StringComparison.OrdinalIgnoreCase))
            throw new UnauthorizedAccessException("Tentativa de acesso fora da área de uploads");
        return completo;
    }

    private static async Task<bool> AssinaturaValidaAsync(Stream stream, string extensao)
    {
        var header = new byte[12];
        var lidos = await stream.ReadAsync(header);
        return extensao switch
        {
            ".pdf" => lidos >= 5 && header.AsSpan(0, 5).SequenceEqual("%PDF-"u8),
            ".jpg" or ".jpeg" => lidos >= 3 && header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF,
            ".png" => lidos >= 8 && header.AsSpan(0, 8).SequenceEqual(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 }),
            ".gif" => lidos >= 6 && (header.AsSpan(0, 6).SequenceEqual("GIF87a"u8) || header.AsSpan(0, 6).SequenceEqual("GIF89a"u8)),
            ".bmp" => lidos >= 2 && header[0] == (byte)'B' && header[1] == (byte)'M',
            _ => false
        };
    }
}
