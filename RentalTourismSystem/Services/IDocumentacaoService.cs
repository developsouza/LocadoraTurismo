using RentalTourismSystem.Models.ViewModels;

namespace RentalTourismSystem.Services
{
    public interface IDocumentacaoService
    {
        ListaDocumentosViewModel ObterListaDocumentos();
        DocumentoViewModel? ObterDocumento(string id);
        List<ResultadoBuscaDocumentacao> BuscarNaDocumentacao(string termo);
        (byte[]? conteudo, string nomeArquivo) ObterArquivoParaDownload(string id);
        List<DocumentoViewModel> ObterDocumentosPorPerfil(string perfil);
    }
}
