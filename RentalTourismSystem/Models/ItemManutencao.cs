using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentalTourismSystem.Models
{
    /// <summary>
    /// Representa um item individual (pe�a/servi�o) de uma manuten��o
    /// </summary>
    public class ItemManutencao
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "A manuten��o � obrigat�ria")]
        [Display(Name = "Manuten��o")]
        public int ManutencaoVeiculoId { get; set; }

        [Required(ErrorMessage = "A descri��o do item � obrigat�ria")]
        [StringLength(200)]
        [Display(Name = "Descri��o")]
        public string Descricao { get; set; } = string.Empty;

        [Required(ErrorMessage = "O tipo de item � obrigat�rio")]
        [StringLength(20)]
        [Display(Name = "Tipo")]
        public string Tipo { get; set; } = "Pe�a"; // Pe�a, Servi�o, Outros

        [Required(ErrorMessage = "A quantidade � obrigat�ria")]
        [Range(1, 1000, ErrorMessage = "Quantidade deve estar entre 1 e 1000")]
        [Display(Name = "Quantidade")]
        public int Quantidade { get; set; } = 1;

        [Required(ErrorMessage = "O valor unit�rio � obrigat�rio")]
        [Column(TypeName = "decimal(10,2)")]
        [Range(0.01, 99999.99, ErrorMessage = "Valor deve estar entre R$ 0,01 e R$ 99.999,99")]
        [Display(Name = "Valor Unit�rio")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal ValorUnitario { get; set; }

        [StringLength(100)]
        [Display(Name = "Fornecedor")]
        public string? Fornecedor { get; set; }

        [StringLength(50)]
        [Display(Name = "C�digo/N�mero de Pe�a")]
        public string? CodigoPeca { get; set; }

        [StringLength(500)]
        [Display(Name = "Observa��es")]
        public string? Observacoes { get; set; }

        // Navega��o
        [Display(Name = "Manuten��o")]
        public virtual ManutencaoVeiculo? ManutencaoVeiculo { get; set; }

        // Propriedades calculadas
        [NotMapped]
        [Display(Name = "Valor Total")]
        public decimal ValorTotal => Quantidade * ValorUnitario;
    }
}
