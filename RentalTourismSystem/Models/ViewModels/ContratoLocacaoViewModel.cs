namespace RentalTourismSystem.Models.ViewModels
{
    public class ContratoLocacaoViewModel
    {
        public Locacao Locacao { get; set; }
        public int DiasLocacao { get; set; }
        public decimal ValorDiaria { get; set; }
        public decimal ValorCaucao { get; set; }
        public decimal ValorFranquia { get; set; }
        public DateTime DataEmissao { get; set; }
    }
}
