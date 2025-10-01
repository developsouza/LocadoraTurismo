using RentalTourismSystem.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentalTourismSystem.Models
{
    public partial class ReservaViagem
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "A data da reserva é obrigatória")]
        [Display(Name = "Data da Reserva")]
        public DateTime DataReserva { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "A data da viagem é obrigatória")]
        [DataFuturaValidation(ErrorMessage = "A data da viagem deve ser futura")]
        [Display(Name = "Data da Viagem")]
        public DateTime DataViagem { get; set; }

        [Required(ErrorMessage = "A quantidade de pessoas é obrigatória")]
        [Range(1, 50, ErrorMessage = "A quantidade deve estar entre 1 e 50 pessoas")]
        [Display(Name = "Quantidade de Pessoas")]
        public int Quantidade { get; set; } = 1;

        [Required(ErrorMessage = "O valor total é obrigatório")]
        [Column(TypeName = "decimal(10,2)")]
        [Range(0.01, 999999.99, ErrorMessage = "O valor deve ser maior que zero")]
        [Display(Name = "Valor Total")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal ValorTotal { get; set; }

        [StringLength(500, ErrorMessage = "As observações devem ter no máximo 500 caracteres")]
        [Display(Name = "Observações")]
        public string? Observacoes { get; set; }

        [Required(ErrorMessage = "O cliente é obrigatório")]
        [Display(Name = "Cliente")]
        public int ClienteId { get; set; }

        [Required(ErrorMessage = "O pacote de viagem é obrigatório")]
        [Display(Name = "Pacote de Viagem")]
        public int PacoteViagemId { get; set; }

        [Required(ErrorMessage = "O status da reserva é obrigatório")]
        [Display(Name = "Status da Reserva")]
        public int StatusReservaViagemId { get; set; } = 1; // Pendente por padrão

        [Display(Name = "Data de Criação")]
        public DateTime DataCriacao { get; set; } = DateTime.Now;

        [Display(Name = "Última Atualização")]
        public DateTime? UltimaAtualizacao { get; set; }

        // Propriedades calculadas
        [Display(Name = "Valor por Pessoa")]
        public decimal ValorPorPessoa => Quantidade > 0 ? ValorTotal / Quantidade : 0;

        [Display(Name = "Dias até a Viagem")]
        public int DiasAteViagem => (int)Math.Max(0, (DataViagem.Date - DateTime.Now.Date).TotalDays);

        [Display(Name = "É Viagem Futura")]
        public bool EhViagemFutura => DataViagem.Date > DateTime.Now.Date;

        [Display(Name = "É Viagem Hoje")]
        public bool EhViagemHoje => DataViagem.Date == DateTime.Now.Date;

        [Display(Name = "Viagem Realizada")]
        public bool ViagemRealizada => DataViagem.Date < DateTime.Now.Date;

        [Display(Name = "Status Atual")]
        public string StatusAtual => ViagemRealizada && StatusReservaViagem?.Status == "Confirmada"
            ? "Realizada"
            : StatusReservaViagem?.Status ?? "Indefinido";

        [Display(Name = "Pode Cancelar")]
        public bool PodeCancelar => EhViagemFutura &&
            (StatusReservaViagem?.Status == "Pendente" || StatusReservaViagem?.Status == "Confirmada") &&
            DiasAteViagem > 1; // Permite cancelar até 1 dia antes

        [Display(Name = "Pode Confirmar")]
        public bool PodeConfirmar => EhViagemFutura && StatusReservaViagem?.Status == "Pendente";

        [Display(Name = "Precisa Atenção")]
        public bool PrecisaAtencao => (StatusReservaViagem?.Status == "Pendente" && DiasAteViagem <= 7) ||
            (StatusReservaViagem?.Status == "Confirmada" && DiasAteViagem <= 3);

        [Display(Name = "Tempo desde Reserva")]
        public int DiasDaReserva => (int)(DateTime.Now.Date - DataReserva.Date).TotalDays;

        [Display(Name = "Reserva Recente")]
        public bool ReservaRecente => DiasDaReserva <= 7;

        [Display(Name = "Total com Serviços")]
        public decimal ValorTotalComServicos => ValorTotal + (ServicosAdicionais?.Sum(s => s.Preco) ?? 0);

        [Display(Name = "Tem Serviços Adicionais")]
        public bool TemServicosAdicionais => ServicosAdicionais?.Any() == true;

        [Display(Name = "Quantidade de Serviços")]
        public int QuantidadeServicos => ServicosAdicionais?.Count ?? 0;

        // Navigation properties
        [Display(Name = "Cliente")]
        public virtual Cliente Cliente { get; set; } = null!;

        [Display(Name = "Pacote de Viagem")]
        public virtual PacoteViagem PacoteViagem { get; set; } = null!;

        [Display(Name = "Status da Reserva")]
        public virtual StatusReservaViagem StatusReservaViagem { get; set; } = null!;

        [Display(Name = "Serviços Adicionais")]
        public virtual ICollection<ServicoAdicional> ServicosAdicionais { get; set; } = new List<ServicoAdicional>();

        // Métodos auxiliares
        public bool PodeSerEditada()
        {
            return StatusReservaViagem?.Status == "Pendente" && EhViagemFutura && DiasAteViagem > 1;
        }

        public bool PodeSerExcluida()
        {
            return StatusReservaViagem?.Status == "Pendente" && EhViagemFutura && DiasAteViagem > 3;
        }

        public string ObterClasseStatusBadge()
        {
            return StatusReservaViagem?.Status switch
            {
                "Pendente" => "bg-warning",
                "Confirmada" => "bg-success",
                "Cancelada" => "bg-danger",
                "Realizada" => "bg-info",
                _ => "bg-secondary"
            };
        }

        public string ObterDescricaoDetalhada()
        {
            var cliente = Cliente?.Nome ?? "Cliente não informado";
            var pacote = PacoteViagem?.Nome ?? "Pacote não informado";
            var data = DataViagem.ToString("dd/MM/yyyy");
            var pessoas = Quantidade == 1 ? "1 pessoa" : $"{Quantidade} pessoas";

            return $"{cliente} - {pacote} em {data} para {pessoas}";
        }

        public decimal CalcularDesconto(decimal percentualDesconto)
        {
            return ValorTotal * (percentualDesconto / 100);
        }

        public decimal CalcularValorComDesconto(decimal percentualDesconto)
        {
            return ValorTotal - CalcularDesconto(percentualDesconto);
        }

        public void AtualizarUltimaModificacao()
        {
            UltimaAtualizacao = DateTime.Now;
        }

        public bool ValidarDataViagem()
        {
            return DataViagem.Date > DateTime.Now.Date;
        }

        public bool ValidarQuantidade()
        {
            return Quantidade > 0 && Quantidade <= 50;
        }

        public string ObterMensagemValidacao()
        {
            var erros = new List<string>();

            if (!ValidarDataViagem())
                erros.Add("A data da viagem deve ser futura");

            if (!ValidarQuantidade())
                erros.Add("A quantidade deve estar entre 1 e 50 pessoas");

            if (ValorTotal <= 0)
                erros.Add("O valor total deve ser maior que zero");

            if (ClienteId <= 0)
                erros.Add("É necessário selecionar um cliente");

            if (PacoteViagemId <= 0)
                erros.Add("É necessário selecionar um pacote de viagem");

            return erros.Any() ? string.Join("; ", erros) : string.Empty;
        }
    }
}