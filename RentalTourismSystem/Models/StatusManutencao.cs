using System.ComponentModel.DataAnnotations;

namespace RentalTourismSystem.Models
{
    /// <summary>
    /// Representa o status de uma manutenção veicular
    /// </summary>
    public class StatusManutencao
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O status é obrigatório")]
        [StringLength(30)]
        [Display(Name = "Status")]
        public string Status { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "Descrição")]
        public string? Descricao { get; set; }

        // Navegação
        public virtual ICollection<ManutencaoVeiculo> Manutencoes { get; set; } = new List<ManutencaoVeiculo>();
    }
}
