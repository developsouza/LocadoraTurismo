using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentalTourismSystem.Models
{
    public class PacoteViagem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Nome do Pacote")]
        public string Nome { get; set; }

        [Required]
        [StringLength(1000)]
        [Display(Name = "Descrição")]
        public string Descricao { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Destino")]
        public string Destino { get; set; }

        [Required]
        [Display(Name = "Duração")]
        public int Duracao { get; set; }

        [Required]
        [Display(Name = "Unidade de Tempo")]
        public string UnidadeTempo { get; set; } = "dias"; // "dias" ou "horas"

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        [Display(Name = "Preço por Pessoa")]
        public decimal Preco { get; set; }

        [Display(Name = "Ativo")]
        public bool Ativo { get; set; } = true;

        [Display(Name = "Data de Criação")]
        public DateTime DataCriacao { get; set; } = DateTime.Now;

        public virtual ICollection<ReservaViagem> ReservasViagens { get; set; } = new List<ReservaViagem>();

        // Propriedades calculadas
        [NotMapped]
        [Display(Name = "Duração Formatada")]
        public string DuracaoFormatada => $"{Duracao} {UnidadeTempo}";

        [NotMapped]
        [Display(Name = "Total de Reservas")]
        public int TotalReservas => ReservasViagens?.Count ?? 0;

        [NotMapped]
        [Display(Name = "Receita Total")]
        public decimal ReceitaTotal => ReservasViagens?
            .Where(r => r.StatusReservaViagem?.Status == "Confirmada" || r.StatusReservaViagem?.Status == "Realizada")
            .Sum(r => r.ValorTotal) ?? 0;

        [NotMapped]
        [Display(Name = "Última Reserva")]
        public DateTime? UltimaReserva => ReservasViagens?
            .Where(r => r.DataReserva != null)
            .OrderByDescending(r => r.DataReserva)
            .FirstOrDefault()?.DataReserva;
    }
}