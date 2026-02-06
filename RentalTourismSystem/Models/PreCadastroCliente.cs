using RentalTourismSystem.Helpers;
using System.ComponentModel.DataAnnotations;

namespace RentalTourismSystem.Models
{
    /// <summary>
    /// Modelo para pré-cadastro público de clientes
    /// </summary>
    public class PreCadastroCliente
    {
        [Required(ErrorMessage = "O nome completo é obrigatório")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 100 caracteres")]
        [Display(Name = "Nome Completo")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O CPF é obrigatório")]
        [CpfValidation(ErrorMessage = "CPF inválido")]
        [StringLength(14)]
        [Display(Name = "CPF")]
        public string CPF { get; set; } = string.Empty;

        [Required(ErrorMessage = "O telefone é obrigatório")]
        [StringLength(20)]
        [Display(Name = "Telefone")]
        [RegularExpression(@"^\(\d{2}\)\s?\d{4,5}-?\d{4}$", ErrorMessage = "Telefone inválido. Use o formato (00) 00000-0000")]
        public string Telefone { get; set; } = string.Empty;

        [Required(ErrorMessage = "O upload da CNH é obrigatório")]
        [Display(Name = "Foto da CNH")]
        public IFormFile? CNHUpload { get; set; }

        [Required(ErrorMessage = "A data inicial da locação é obrigatória")]
        [DataType(DataType.Date)]
        [Display(Name = "Data Inicial da Locação")]
        public DateTime DataInicioLocacao { get; set; } = DateTime.Now.AddDays(1);

        [Required(ErrorMessage = "A data final da locação é obrigatória")]
        [DataType(DataType.Date)]
        [Display(Name = "Data Final da Locação")]
        [DataMaiorQue("DataInicioLocacao", ErrorMessage = "A data final deve ser posterior à data inicial")]
        public DateTime DataFinalLocacao { get; set; } = DateTime.Now.AddDays(8);

        [Required(ErrorMessage = "Selecione um veículo")]
        [Display(Name = "Veículo Desejado")]
        [Range(1, int.MaxValue, ErrorMessage = "Selecione um veículo válido")]
        public int VeiculoId { get; set; }

        [Required(ErrorMessage = "Você deve confirmar que não é um robô")]
        [Display(Name = "Não sou um robô")]
        [Range(typeof(bool), "true", "true", ErrorMessage = "Você deve confirmar que não é um robô")]
        public bool ConfirmarHumano { get; set; }

        // Propriedades calculadas
        [Display(Name = "Quantidade de Dias")]
        public int QuantidadeDias => (DataFinalLocacao - DataInicioLocacao).Days;
    }

    /// <summary>
    /// Validação customizada para verificar se uma data é maior que outra
    /// </summary>
    public class DataMaiorQueAttribute : ValidationAttribute
    {
        private readonly string _dataInicialProperty;

        public DataMaiorQueAttribute(string dataInicialProperty)
        {
            _dataInicialProperty = dataInicialProperty;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var dataFinal = (DateTime?)value;
            var propertyInfo = validationContext.ObjectType.GetProperty(_dataInicialProperty);

            if (propertyInfo == null)
                return new ValidationResult($"Propriedade {_dataInicialProperty} não encontrada");

            var dataInicial = (DateTime?)propertyInfo.GetValue(validationContext.ObjectInstance);

            if (dataFinal.HasValue && dataInicial.HasValue && dataFinal.Value <= dataInicial.Value)
            {
                return new ValidationResult(ErrorMessage ?? "A data final deve ser posterior à data inicial");
            }

            return ValidationResult.Success;
        }
    }
}
