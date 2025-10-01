using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [Required]
        public int ClienteId { get; set; }
        public virtual Cliente Cliente { get; set; }

        [Required]
        public int VeiculoId { get; set; }
        public virtual Veiculo Veiculo { get; set; }

        [Required]
        public int FuncionarioId { get; set; }
        public virtual Funcionario Funcionario { get; set; }

        [Required]
        public int AgenciaId { get; set; }
        public virtual Agencia Agencia { get; set; }
        public int QuilometragemDevolucao { get; internal set; }
    }
}