namespace RentalTourismSystem.Models.ViewModels
{
    public class LaudoVistoriaViewModel
    {
        public Locacao Locacao { get; set; } = null!;
        public DateTime DataVistoria { get; set; }
        public List<ItemVistoria> ItensVistoria { get; set; } = [];
        public string Observacoes { get; set; } = string.Empty;
    }
}
