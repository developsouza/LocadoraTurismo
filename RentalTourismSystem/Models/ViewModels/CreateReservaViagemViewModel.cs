using System.ComponentModel.DataAnnotations;

namespace RentalTourismSystem.Models.ViewModels
{
    public class CreateReservaViagemViewModel
    {
        [Required(ErrorMessage = "A data da reserva é obrigatória")]
        [Display(Name = "Data da Reserva")]
        [DataType(DataType.DateTime)]
        public DateTime DataReserva { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "A data da viagem é obrigatória")]
        [Display(Name = "Data da Viagem")]
        [DataType(DataType.Date)]
        [DateGreaterThan("DataReserva", ErrorMessage = "A data da viagem deve ser posterior à data da reserva")]
        public DateTime DataViagem { get; set; } = DateTime.Now.AddDays(7);

        [Required(ErrorMessage = "A quantidade de pessoas é obrigatória")]
        [Display(Name = "Quantidade de Pessoas")]
        [Range(1, 50, ErrorMessage = "A quantidade deve ser entre 1 e 50 pessoas")]
        public int Quantidade { get; set; } = 1;

        [Display(Name = "Observações")]
        [StringLength(500, ErrorMessage = "As observações não podem exceder 500 caracteres")]
        public string? Observacoes { get; set; }

        [Required(ErrorMessage = "Selecione um cliente")]
        [Display(Name = "Cliente")]
        public int ClienteId { get; set; }

        [Required(ErrorMessage = "Selecione um pacote de viagem")]
        [Display(Name = "Pacote de Viagem")]
        public int PacoteViagemId { get; set; }

        // Propriedades calculadas/exibição
        [Display(Name = "Valor Estimado")]
        [DataType(DataType.Currency)]
        public decimal? ValorEstimado { get; set; }
    }

    // Validação customizada para data
    public class DateGreaterThanAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;

        public DateGreaterThanAttribute(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var currentValue = (DateTime)value;
            var property = validationContext.ObjectType.GetProperty(_comparisonProperty);

            if (property == null)
                throw new ArgumentException("Property with this name not found");

            var comparisonValue = (DateTime)property.GetValue(validationContext.ObjectInstance);

            if (currentValue <= comparisonValue)
                return new ValidationResult(ErrorMessage);

            return ValidationResult.Success;
        }
    }
}