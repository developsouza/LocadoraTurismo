using System.ComponentModel.DataAnnotations;

namespace RentalTourismSystem.Models
{
    public class StatusCarro
    {
        public const int DisponivelId = 1;
        public const int AlugadoId = 2;
        public const int ManutencaoId = 3;
        public const int IndisponivelId = 4;
        public const int ReservadoId = 5;

        public const string Disponivel = "Disponível";
        public const string Alugado = "Alugado";
        public const string Manutencao = "Manutenção";
        public const string Indisponivel = "Indisponível";
        public const string Reservado = "Reservado";

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = string.Empty;

        public virtual ICollection<Veiculo> Veiculos { get; set; } = new List<Veiculo>();
    }
}
