using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace RentalTourismSystem.Models
{
    public class Locacao
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime DataRetirada { get; set; }

        [Required]
        public DateTime DataDevolucao { get; set; }

        public DateTime? DataDevolucaoReal { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal ValorTotal { get; set; }

        [StringLength(500)]
        public string? Observacoes { get; set; }

        // Quilometragens
        [NotMapped]
        [Range(0, int.MaxValue)]
        public int QuilometragemRetirada { get; set; }

        public int QuilometragemDevolucao { get; set; }

        // IDs obrigatórios e não podem ser 0
        [Range(1, int.MaxValue, ErrorMessage = "Selecione um cliente válido")]
        public int ClienteId { get; set; }

        [ValidateNever]
        public virtual Cliente? Cliente { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Selecione um veículo válido")]
        public int VeiculoId { get; set; }

        [ValidateNever]
        public virtual Veiculo? Veiculo { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Selecione um funcionário válido")]
        public int FuncionarioId { get; set; }

        [ValidateNever]
        public virtual Funcionario? Funcionario { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Selecione uma agência válida")]
        public int AgenciaId { get; set; }

        [ValidateNever]
        public virtual Agencia? Agencia { get; set; }
    }
}