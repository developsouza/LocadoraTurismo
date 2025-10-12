namespace RentalTourismSystem.Models.ViewModels
{
    public class DocumentoViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string ConteudoHtml { get; set; } = string.Empty;
        public string ConteudoMarkdown { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public string Icone { get; set; } = "fas fa-file-alt";
        public int TempoLeitura { get; set; } // Em minutos
        public List<string> Tags { get; set; } = new();
        public List<string> PerfisSugeridos { get; set; } = new(); // Admin, Manager, Employee
        public DateTime? UltimaAtualizacao { get; set; }
    }

    public class ListaDocumentosViewModel
    {
        public List<DocumentoViewModel> Documentos { get; set; } = new();
        public Dictionary<string, List<DocumentoViewModel>> DocumentosPorCategoria { get; set; } = new();
    }

    public class ResultadoBuscaDocumentacao
    {
        public string DocumentoId { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string TrechoDestacado { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public int Relevancia { get; set; }
    }
}
