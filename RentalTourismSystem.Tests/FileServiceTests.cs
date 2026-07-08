using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging.Abstractions;
using RentalTourismSystem.Services;

namespace RentalTourismSystem.Tests;

public sealed class FileServiceTests : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), $"rental-tests-{Guid.NewGuid():N}");
    private readonly FileService _service;

    public FileServiceTests()
    {
        Directory.CreateDirectory(_root);
        _service = new FileService(new TestEnvironment(_root), NullLogger<FileService>.Instance);
    }

    [Fact]
    public async Task SalvarArquivoAsync_DeveSalvarPdfValidoForaDoWebRoot()
    {
        var pdf = CriarArquivo("documento.pdf", "%PDF-1.7\nconteudo"u8.ToArray());

        var result = await _service.SalvarArquivoAsync(pdf, "clientes/42", [".pdf"]);

        Assert.True(result.Success, result.ErrorMessage);
        Assert.DoesNotContain("wwwroot", result.FilePath, StringComparison.OrdinalIgnoreCase);
        Assert.True(File.Exists(_service.ObterCaminhoCompleto(result.FilePath)));
    }

    [Fact]
    public async Task SalvarArquivoAsync_DeveRejeitarExecutavelDisfarcadoDePdf()
    {
        var arquivo = CriarArquivo("malicioso.pdf", "MZ-executavel"u8.ToArray());

        var result = await _service.SalvarArquivoAsync(arquivo, "documentos", [".pdf"]);

        Assert.False(result.Success);
        Assert.Contains("conteúdo", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("../appsettings.json")]
    [InlineData("uploads/../../segredo.txt")]
    [InlineData("..\\..\\segredo.txt")]
    public void ObterCaminhoCompleto_DeveBloquearPathTraversal(string caminho)
    {
        Assert.Throws<UnauthorizedAccessException>(() => _service.ObterCaminhoCompleto(caminho));
    }

    [Fact]
    public async Task ObterArquivoAsync_DeveRetornarTipoDerivadoDoArquivo()
    {
        var pngBytes = new byte[] { 137, 80, 78, 71, 13, 10, 26, 10, 0, 0, 0, 0 };
        var saved = await _service.SalvarArquivoAsync(CriarArquivo("foto.png", pngBytes), "cnh", [".png"]);

        var result = await _service.ObterArquivoAsync(saved.FilePath);

        Assert.True(result.Success);
        Assert.Equal("image/png", result.ContentType);
        Assert.Equal(pngBytes, result.FileBytes);
    }

    private static FormFile CriarArquivo(string nome, byte[] bytes) =>
        new(new MemoryStream(bytes), 0, bytes.Length, "arquivo", nome);

    public void Dispose()
    {
        if (Directory.Exists(_root)) Directory.Delete(_root, recursive: true);
    }

    private sealed class TestEnvironment(string root) : IWebHostEnvironment
    {
        public string ApplicationName { get; set; } = "Tests";
        public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
        public string WebRootPath { get; set; } = Path.Combine(root, "wwwroot");
        public string EnvironmentName { get; set; } = "Development";
        public string ContentRootPath { get; set; } = root;
        public IFileProvider ContentRootFileProvider { get; set; } = new PhysicalFileProvider(root);
    }
}
