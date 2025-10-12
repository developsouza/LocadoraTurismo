using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentalTourismSystem.Models
{
    /// <summary>
    /// Representa um item individual (peça/serviço) de uma manutenção
    /// </summary>
    public class ItemManutencao
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "A manutenção é obrigatória")]
        [Display(Name = "Manutenção")]
        public int ManutencaoVeiculoId { get; set; }

        [Required(ErrorMessage = "A descrição do item é obrigatória")]
        [StringLength(200)]
        [Display(Name = "Descrição")]
        public string Descricao { get; set; } = string.Empty;

        [Required(ErrorMessage = "O tipo de item é obrigatório")]
        [StringLength(20)]
        [Display(Name = "Tipo")]
        public string Tipo { get; set; } = "Peça"; // Peça, Serviço, Outros

        [Required(ErrorMessage = "A quantidade é obrigatória")]
        [Range(1, 1000, ErrorMessage = "Quantidade deve estar entre 1 e 1000")]
        [Display(Name = "Quantidade")]
        public int Quantidade { get; set; } = 1;

        [Required(ErrorMessage = "O valor unitário é obrigatório")]
        [Column(TypeName = "decimal(10,2)")]
        [Range(0.01, 99999.99, ErrorMessage = "Valor deve estar entre R$ 0,01 e R$ 99.999,99")]
        [Display(Name = "Valor Unitário")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal ValorUnitario { get; set; }

        [StringLength(100)]
        [Display(Name = "Fornecedor")]
        public string? Fornecedor { get; set; }

        [StringLength(50)]
        [Display(Name = "Código/Número de Peça")]
        public string? CodigoPeca { get; set; }

        [StringLength(500)]
        [Display(Name = "Observações")]
        public string? Observacoes { get; set; }

        // Navegação
        [Display(Name = "Manutenção")]
        public virtual ManutencaoVeiculo? ManutencaoVeiculo { get; set; }

        // Propriedades calculadas
        [NotMapped]
        [Display(Name = "Valor Total")]
        public decimal ValorTotal => Quantidade * ValorUnitario;
    }
}
