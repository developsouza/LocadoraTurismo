using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentalTourismSystem.Models
{
    /// <summary>
    /// Representa uma manutenção realizada ou agendada em um veículo
    /// </summary>
    public class ManutencaoVeiculo
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O veículo é obrigatório")]
        [Display(Name = "Veículo")]
        public int VeiculoId { get; set; }

        [Required(ErrorMessage = "O tipo de manutenção é obrigatório")]
        [Display(Name = "Tipo de Manutenção")]
        public int TipoManutencaoId { get; set; }

        [Required(ErrorMessage = "O status é obrigatório")]
        [Display(Name = "Status")]
        public int StatusManutencaoId { get; set; }

        [Required(ErrorMessage = "A data de agendamento é obrigatória")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Data Agendada")]
        public DateTime DataAgendada { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Data de Início")]
        public DateTime? DataInicio { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Data de Conclusão")]
        public DateTime? DataConclusao { get; set; }

        [Required(ErrorMessage = "A quilometragem atual é obrigatória")]
        [Range(0, int.MaxValue, ErrorMessage = "Quilometragem deve ser maior ou igual a zero")]
        [Display(Name = "Quilometragem Atual")]
        public int QuilometragemAtual { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Quilometragem deve ser maior ou igual a zero")]
        [Display(Name = "Próxima Quilometragem")]
        public int? ProximaQuilometragem { get; set; }

        [Required(ErrorMessage = "A descrição é obrigatória")]
        [StringLength(500)]
        [Display(Name = "Descrição")]
        public string Descricao { get; set; } = string.Empty;

        [StringLength(1000)]
        [Display(Name = "Observações")]
        public string? Observacoes { get; set; }

        [StringLength(200)]
        [Display(Name = "Oficina/Prestador")]
        public string? Oficina { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [Range(0, 999999.99, ErrorMessage = "O custo deve estar entre R$ 0,00 e R$ 999.999,99")]
        [Display(Name = "Custo da Manutenção")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal? Custo { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [Range(0, 999999.99, ErrorMessage = "O valor de peças deve estar entre R$ 0,00 e R$ 999.999,99")]
        [Display(Name = "Custo de Peças")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal? CustoPecas { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [Range(0, 999999.99, ErrorMessage = "O valor de mão de obra deve estar entre R$ 0,00 e R$ 999.999,99")]
        [Display(Name = "Custo de Mão de Obra")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal? CustoMaoObra { get; set; }

        [Display(Name = "Garantia (dias)")]
        [Range(0, 3650, ErrorMessage = "Garantia deve estar entre 0 e 3650 dias (10 anos)")]
        public int? GarantiaDias { get; set; }

        [Display(Name = "Manutenção Preventiva")]
        public bool Preventiva { get; set; } = false;

        [Display(Name = "Urgente")]
        public bool Urgente { get; set; } = false;

        [Display(Name = "Responsável")]
        public int? FuncionarioId { get; set; }

        [Display(Name = "Data de Cadastro")]
        public DateTime DataCadastro { get; set; } = DateTime.Now;

        [StringLength(200)]
        [Display(Name = "Nota Fiscal")]
        public string? NotaFiscal { get; set; }

        // Navegação
        [Display(Name = "Veículo")]
        public virtual Veiculo? Veiculo { get; set; }

        [Display(Name = "Tipo de Manutenção")]
        public virtual TipoManutencao? TipoManutencao { get; set; }

        [Display(Name = "Status")]
        public virtual StatusManutencao? StatusManutencao { get; set; }

        [Display(Name = "Responsável")]
        public virtual Funcionario? Funcionario { get; set; }

        public virtual ICollection<ItemManutencao> Itens { get; set; } = new List<ItemManutencao>();

        // Propriedades calculadas
        [NotMapped]
        [Display(Name = "Custo Total")]
        public decimal CustoTotal => (Custo ?? 0) + (CustoPecas ?? 0) + (CustoMaoObra ?? 0);

        [NotMapped]
        [Display(Name = "Dias Agendados")]
        public int DiasAteAgendamento => (DataAgendada.Date - DateTime.Now.Date).Days;

        [NotMapped]
        [Display(Name = "Duração (horas)")]
        public double? DuracaoHoras
        {
            get
            {
                if (DataInicio.HasValue && DataConclusao.HasValue)
                {
                    return (DataConclusao.Value - DataInicio.Value).TotalHours;
                }
                return null;
            }
        }

        [NotMapped]
        [Display(Name = "Em Garantia")]
        public bool EmGarantia
        {
            get
            {
                if (DataConclusao.HasValue && GarantiaDias.HasValue)
                {
                    var dataFimGarantia = DataConclusao.Value.AddDays(GarantiaDias.Value);
                    return DateTime.Now <= dataFimGarantia;
                }
                return false;
            }
        }

        [NotMapped]
        [Display(Name = "Dias Restantes Garantia")]
        public int? DiasRestantesGarantia
        {
            get
            {
                if (EmGarantia && DataConclusao.HasValue && GarantiaDias.HasValue)
                {
                    var dataFimGarantia = DataConclusao.Value.AddDays(GarantiaDias.Value);
                    return (dataFimGarantia.Date - DateTime.Now.Date).Days;
                }
                return null;
            }
        }
    }
}
