using RentalTourismSystem.Helpers;
using System.ComponentModel.DataAnnotations;

namespace RentalTourismSystem.Models
{
    public partial class Cliente
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
        [Display(Name = "Nome Completo")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O CPF é obrigatório")]
        [CpfValidation]
        [StringLength(14)]
        [Display(Name = "CPF")]
        public string Cpf { get; set; }

        // Removido Required do telefone se não for sempre obrigatório
        [StringLength(20)]
        [RegularExpression(@"^\(\d{2}\)\s\d{4,5}-\d{4}$",
            ErrorMessage = "Telefone deve estar no formato (11) 99999-9999")]
        [Display(Name = "Telefone")]
        public string? Telefone { get; set; }

        // Email pode ser opcional dependendo da regra de negócio
        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(100)]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "O endereço é obrigatório")]
        [StringLength(200)]
        [Display(Name = "Endereço")]
        public string Endereco { get; set; }

        [Required(ErrorMessage = "A data de nascimento é obrigatória")]
        [DataType(DataType.Date)]
        [IdadeValidation(21, 100, ErrorMessage = "Cliente deve ter entre 21 e 100 anos")]
        [Display(Name = "Data de Nascimento")]
        public DateTime DataNascimento { get; set; }

        [StringLength(20, ErrorMessage = "Número da CNH deve ter no máximo 20 caracteres")]
        [Display(Name = "Número da CNH")]
        public string? NumeroHabilitacao { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Validade da CNH")]
        [ValidadeCNH(ErrorMessage = "CNH deve estar válida")]
        public DateTime? ValidadeCNH { get; set; }

        [Display(Name = "Data de Cadastro")]
        public DateTime DataCadastro { get; set; } = DateTime.Now;

        // Propriedades calculadas
        [Display(Name = "Idade")]
        public int Idade => DateTime.Now.Year - DataNascimento.Year -
            (DateTime.Now.DayOfYear < DataNascimento.DayOfYear ? 1 : 0);

        [Display(Name = "CNH Válida")]
        public bool CNHValida => !string.IsNullOrEmpty(NumeroHabilitacao) &&
                                ValidadeCNH.HasValue &&
                                ValidadeCNH.Value.Date >= DateTime.Now.Date;

        [Display(Name = "CNH Vence em")]
        public int? DiasParaVencimentoCNH => ValidadeCNH.HasValue
            ? (int)(ValidadeCNH.Value - DateTime.Now.Date).TotalDays
            : null;

        [Display(Name = "Total de Locações")]
        public int TotalLocacoes => Locacoes?.Count ?? 0;

        [Display(Name = "Total de Reservas")]
        public int TotalReservas => ReservasViagens?.Count ?? 0;

        [Display(Name = "Valor Total Gasto")]
        public decimal ValorTotalGasto => (Locacoes?.Sum(l => l.ValorTotal) ?? 0) +
                                         (ReservasViagens?.Sum(r => r.ValorTotal) ?? 0);

        // Navigation properties
        public virtual ICollection<Locacao> Locacoes { get; set; } = new List<Locacao>();
        public virtual ICollection<ReservaViagem> ReservasViagens { get; set; } = new List<ReservaViagem>();
    }

    public class ValidadeCNHAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is DateTime validadeCNH)
            {
                return validadeCNH.Date >= DateTime.Now.Date;
            }
            return true; // Permite null se não for obrigatório
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var cliente = validationContext.ObjectInstance as Cliente;

            // Se tem número da CNH, deve ter validade
            if (!string.IsNullOrEmpty(cliente?.NumeroHabilitacao))
            {
                if (!cliente.ValidadeCNH.HasValue)
                {
                    return new ValidationResult("Validade da CNH é obrigatória quando número da CNH é informado");
                }

                if (cliente.ValidadeCNH.Value.Date < DateTime.Now.Date)
                {
                    return new ValidationResult("CNH está vencida");
                }
            }

            return ValidationResult.Success;
        }
    }
}