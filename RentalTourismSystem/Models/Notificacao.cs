using System.ComponentModel.DataAnnotations;

namespace RentalTourismSystem.Models
{
    public class Notificacao
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Titulo { get; set; } = string.Empty;

        [Required]
        [MaxLength(1000)]
        public string Mensagem { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Tipo { get; set; } = "info"; // info, success, warning, danger/error

        [MaxLength(100)]
        public string? Categoria { get; set; } // CNH, Locacao, Veiculo, Reserva, Sistema

        public string? LinkAcao { get; set; }
        public string? TextoLinkAcao { get; set; }

        public int? ClienteId { get; set; }
        public int? VeiculoId { get; set; }
        public int? LocacaoId { get; set; }
        public int? ReservaId { get; set; }

        public bool Lida { get; set; } = false;
        public DateTime DataCriacao { get; set; } = DateTime.Now;
        public DateTime? DataLeitura { get; set; }

        public int Prioridade { get; set; } = 1; // 1=Normal, 2=Alta, 3=Urgente

        // Navegação
        public virtual Cliente? Cliente { get; set; }
        public virtual Veiculo? Veiculo { get; set; }
        public virtual Locacao? Locacao { get; set; }
        public virtual ReservaViagem? Reserva { get; set; }
    }

    public class NotificacaoResumoDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Mensagem { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string? Categoria { get; set; }
        public string? LinkAcao { get; set; }
        public string? TextoLinkAcao { get; set; }
        public bool Lida { get; set; }
        public DateTime DataCriacao { get; set; }
        public int Prioridade { get; set; }
        public string TempoDecorrido { get; set; } = string.Empty;
    }
}