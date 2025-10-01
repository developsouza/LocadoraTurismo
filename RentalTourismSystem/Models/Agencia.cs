using System.ComponentModel.DataAnnotations;

namespace RentalTourismSystem.Models
{
    public class Agencia
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; }

        [Required]
        [StringLength(200)]
        public string Endereco { get; set; }

        [Required]
        [StringLength(20)]
        public string Telefone { get; set; }

        [Required]
        [StringLength(100)]
        public string Email { get; set; }

        public virtual ICollection<Funcionario> Funcionarios { get; set; } = new List<Funcionario>();
        public virtual ICollection<Veiculo> Veiculos { get; set; } = new List<Veiculo>();
        public virtual ICollection<Locacao> Locacoes { get; set; } = new List<Locacao>();
    }
}