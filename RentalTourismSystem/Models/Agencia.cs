using System.ComponentModel.DataAnnotations;

namespace RentalTourismSystem.Models
{
    public class Agencia
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100)]
        [Display(Name = "Nome da Agência")]
        public string Nome { get; set; }

        [StringLength(20)]
        [Display(Name = "CNPJ")]
        public string? CNPJ { get; set; }

        [Required(ErrorMessage = "O endereço é obrigatório")]
        [StringLength(200)]
        [Display(Name = "Endereço")]
        public string Endereco { get; set; }

        [StringLength(100)]
        [Display(Name = "Cidade")]
        public string? Cidade { get; set; }

        [Required(ErrorMessage = "O telefone é obrigatório")]
        [StringLength(20)]
        [Display(Name = "Telefone")]
        public string Telefone { get; set; }

        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(100)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        public virtual ICollection<Funcionario> Funcionarios { get; set; } = new List<Funcionario>();
        public virtual ICollection<Veiculo> Veiculos { get; set; } = new List<Veiculo>();
        public virtual ICollection<Locacao> Locacoes { get; set; } = new List<Locacao>();
    }
}