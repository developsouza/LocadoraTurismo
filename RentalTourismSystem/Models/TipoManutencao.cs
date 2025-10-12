using System.ComponentModel.DataAnnotations;

namespace RentalTourismSystem.Models
{
    /// <summary>
    /// Representa os tipos de manutenção que podem ser realizadas nos veículos
    /// </summary>
    public class TipoManutencao
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do tipo é obrigatório")]
        [StringLength(50)]
        [Display(Name = "Tipo de Manutenção")]
        public string Nome { get; set; } = string.Empty;

        [StringLength(200)]
        [Display(Name = "Descrição")]
        public string? Descricao { get; set; }

        [Display(Name = "Ativo")]
        public bool Ativo { get; set; } = true;

        // Navegação
        public virtual ICollection<ManutencaoVeiculo> Manutencoes { get; set; } = new List<ManutencaoVeiculo>();
    }
}
