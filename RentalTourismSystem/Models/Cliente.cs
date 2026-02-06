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
        public string CPF { get; set; }

        // ✅ CORRIGIDO: Required adicionado, regex removida (validação no controller)
        [Required(ErrorMessage = "O telefone é obrigatório")]
        [StringLength(20)]
        [Display(Name = "Telefone")]
        public string Telefone { get; set; }

        // ✅ CORRIGIDO: Required adicionado
        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(100)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "O endereço é obrigatório")]
        [StringLength(200)]
        [Display(Name = "Endereço")]
        public string Endereco { get; set; }

        [StringLength(10)]
        [RegularExpression(@"^\d{5}-?\d{3}$", ErrorMessage = "CEP deve estar no formato 00000-000")]
        [Display(Name = "CEP")]
        public string? CEP { get; set; }

        [Required(ErrorMessage = "A data de nascimento é obrigatória")]
        [DataType(DataType.Date)]
        [IdadeValidacao(21, 100, ErrorMessage = "Cliente deve ter entre 21 e 100 anos")]
        [Display(Name = "Data de Nascimento")]
        public DateTime DataNascimento { get; set; }

        [StringLength(50)]
        [Display(Name = "Estado Civil")]
        public string? EstadoCivil { get; set; }

        [StringLength(100)]
        [Display(Name = "Profissão")]
        public string? Profissao { get; set; }

        // ✅ CORRIGIDO: Required REMOVIDO - CNH é opcional
        [StringLength(20, ErrorMessage = "Número da CNH deve ter no máximo 20 caracteres")]
        [Display(Name = "Número da CNH")]
        public string? CNH { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Validade da CNH")]
        [ValidadeCNH(ErrorMessage = "CNH deve estar válida")]
        public DateTime? ValidadeCNH { get; set; }

        [StringLength(5)]
        [Display(Name = "Categoria CNH")]
        public string? CategoriaCNH { get; set; }

        [StringLength(500)]
        [Display(Name = "Caminho da CNH")]
        public string? CNHPath { get; set; }

        [Display(Name = "Data de Cadastro")]
        public DateTime DataCadastro { get; set; } = DateTime.Now;

        // Propriedades calculadas
        [Display(Name = "Idade")]
        public int Idade => DateTime.Now.Year - DataNascimento.Year -
            (DateTime.Now.DayOfYear < DataNascimento.DayOfYear ? 1 : 0);

        [Display(Name = "CNH Válida")]
        public bool CNHValida => !string.IsNullOrEmpty(CNH) &&
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
        public virtual ICollection<Documento> Documentos { get; set; } = new List<Documento>();
    }

    public class ValidadeCNHAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var cliente = validationContext.ObjectInstance as Cliente;

            // ✅ CORRIGIDO: Validação mais clara
            if (!string.IsNullOrWhiteSpace(cliente?.CNH))
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

    public class IdadeValidacaoAttribute : ValidationAttribute
    {
        private readonly int _idadeMinima;
        private readonly int _idadeMaxima;

        public IdadeValidacaoAttribute(int idadeMinima, int idadeMaxima)
        {
            _idadeMinima = idadeMinima;
            _idadeMaxima = idadeMaxima;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateTime dataNascimento)
            {
                var idade = DateTime.Now.Year - dataNascimento.Year;
                if (DateTime.Now.DayOfYear < dataNascimento.DayOfYear) idade--;

                if (idade < _idadeMinima || idade > _idadeMaxima)
                {
                    return new ValidationResult(ErrorMessage ?? $"Idade deve estar entre {_idadeMinima} e {_idadeMaxima} anos");
                }
            }

            return ValidationResult.Success;
        }
    }
}