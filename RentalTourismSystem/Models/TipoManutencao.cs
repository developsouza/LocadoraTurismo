using System.ComponentModel.DataAnnotations;

namespace RentalTourismSystem.Models
{
    /// <summary>
    /// Representa os tipos de manuten��o que podem ser realizadas nos ve�culos
    /// </summary>
    public class TipoManutencao
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do tipo � obrigat�rio")]
        [StringLength(50)]
        [Display(Name = "Tipo de Manuten��o")]
        public string Nome { get; set; } = string.Empty;

        [StringLength(200)]
        [Display(Name = "Descri��o")]
        public string? Descricao { get; set; }

        [Display(Name = "Ativo")]
        public bool Ativo { get; set; } = true;

        // Navega��o
        public virtual ICollection<ManutencaoVeiculo> Manutencoes { get; set; } = new List<ManutencaoVeiculo>();
    }
}
