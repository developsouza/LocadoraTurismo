using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentalTourismSystem.Models
{
    /// <summary>
    /// Representa uma manuten��o realizada ou agendada em um ve�culo
    /// </summary>
    public class ManutencaoVeiculo
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O ve�culo � obrigat�rio")]
        [Display(Name = "Ve�culo")]
        public int VeiculoId { get; set; }

        [Required(ErrorMessage = "O tipo de manuten��o � obrigat�rio")]
        [Display(Name = "Tipo de Manuten��o")]
        public int TipoManutencaoId { get; set; }

        [Required(ErrorMessage = "O status � obrigat�rio")]
        [Display(Name = "Status")]
        public int StatusManutencaoId { get; set; }

        [Required(ErrorMessage = "A data de agendamento � obrigat�ria")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Data Agendada")]
        public DateTime DataAgendada { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Data de In�cio")]
        public DateTime? DataInicio { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Data de Conclus�o")]
        public DateTime? DataConclusao { get; set; }

        [Required(ErrorMessage = "A quilometragem atual � obrigat�ria")]
        [Range(0, int.MaxValue, ErrorMessage = "Quilometragem deve ser maior ou igual a zero")]
        [Display(Name = "Quilometragem Atual")]
        public int QuilometragemAtual { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Quilometragem deve ser maior ou igual a zero")]
        [Display(Name = "Pr�xima Quilometragem")]
        public int? ProximaQuilometragem { get; set; }

        [Required(ErrorMessage = "A descri��o � obrigat�ria")]
        [StringLength(500)]
        [Display(Name = "Descri��o")]
        public string Descricao { get; set; } = string.Empty;

        [StringLength(1000)]
        [Display(Name = "Observa��es")]
        public string? Observacoes { get; set; }

        [StringLength(200)]
        [Display(Name = "Oficina/Prestador")]
        public string? Oficina { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [Range(0, 999999.99, ErrorMessage = "O custo deve estar entre R$ 0,00 e R$ 999.999,99")]
        [Display(Name = "Custo da Manuten��o")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal? Custo { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [Range(0, 999999.99, ErrorMessage = "O valor de pe�as deve estar entre R$ 0,00 e R$ 999.999,99")]
        [Display(Name = "Custo de Pe�as")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal? CustoPecas { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [Range(0, 999999.99, ErrorMessage = "O valor de m�o de obra deve estar entre R$ 0,00 e R$ 999.999,99")]
        [Display(Name = "Custo de M�o de Obra")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal? CustoMaoObra { get; set; }

        [Display(Name = "Garantia (dias)")]
        [Range(0, 3650, ErrorMessage = "Garantia deve estar entre 0 e 3650 dias (10 anos)")]
        public int? GarantiaDias { get; set; }

        [Display(Name = "Manuten��o Preventiva")]
        public bool Preventiva { get; set; } = false;

        [Display(Name = "Urgente")]
        public bool Urgente { get; set; } = false;

        [Display(Name = "Respons�vel")]
        public int? FuncionarioId { get; set; }

        [Display(Name = "Data de Cadastro")]
        public DateTime DataCadastro { get; set; } = DateTime.Now;

        [StringLength(200)]
        [Display(Name = "Nota Fiscal")]
        public string? NotaFiscal { get; set; }

        // Navega��o
        [Display(Name = "Ve�culo")]
        public virtual Veiculo? Veiculo { get; set; }

        [Display(Name = "Tipo de Manuten��o")]
        public virtual TipoManutencao? TipoManutencao { get; set; }

        [Display(Name = "Status")]
        public virtual StatusManutencao? StatusManutencao { get; set; }

        [Display(Name = "Respons�vel")]
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
        [Display(Name = "Dura��o (horas)")]
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
