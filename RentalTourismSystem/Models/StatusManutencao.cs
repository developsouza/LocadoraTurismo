using System.ComponentModel.DataAnnotations;

namespace RentalTourismSystem.Models
{
    /// <summary>
    /// Representa o status de uma manuten��o veicular
    /// </summary>
    public class StatusManutencao
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O status � obrigat�rio")]
        [StringLength(30)]
        [Display(Name = "Status")]
        public string Status { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "Descri��o")]
        public string? Descricao { get; set; }

        // Navega��o
        public virtual ICollection<ManutencaoVeiculo> Manutencoes { get; set; } = new List<ManutencaoVeiculo>();
    }
}
