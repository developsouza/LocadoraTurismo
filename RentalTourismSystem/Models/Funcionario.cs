using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentalTourismSystem.Models
{
    public class Funcionario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; }

        [Required]
        [StringLength(14)]
        public string Cpf { get; set; }

        [Required]
        [StringLength(20)]
        public string Telefone { get; set; }

        [Required]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(50)]
        public string Cargo { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Salario { get; set; }

        [Required]
        public DateTime DataAdmissao { get; set; }

        [Display(Name = "Ativo")]
        public bool Ativo { get; set; } = true;

        [Required]
        public int AgenciaId { get; set; }
        public virtual Agencia Agencia { get; set; }

        public virtual ICollection<Locacao> Locacoes { get; set; } = new List<Locacao>();
    }
}