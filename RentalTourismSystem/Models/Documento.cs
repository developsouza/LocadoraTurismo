using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentalTourismSystem.Models
{
    public class Documento
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Nome do Arquivo")]
        public string NomeArquivo { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        [Display(Name = "Caminho do Arquivo")]
        public string CaminhoArquivo { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Display(Name = "Tipo do Documento")]
        public string TipoDocumento { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "Tipo de Conteúdo")]
        public string? ContentType { get; set; }

        [Display(Name = "Tamanho (bytes)")]
        public long TamanhoBytes { get; set; }

        [StringLength(500)]
        [Display(Name = "Descrição")]
        public string? Descricao { get; set; }

        [Display(Name = "Data de Upload")]
        public DateTime DataUpload { get; set; } = DateTime.Now;

        [StringLength(100)]
        [Display(Name = "Usuário que fez Upload")]
        public string? UsuarioUpload { get; set; }

        // Relacionamentos - Um documento pode pertencer a diferentes entidades
        [Display(Name = "Cliente")]
        public int? ClienteId { get; set; }
        public virtual Cliente? Cliente { get; set; }

        [Display(Name = "Veículo")]
        public int? VeiculoId { get; set; }
        public virtual Veiculo? Veiculo { get; set; }

        [Display(Name = "Funcionário")]
        public int? FuncionarioId { get; set; }
        public virtual Funcionario? Funcionario { get; set; }

        [Display(Name = "Usuário do Sistema")]
        public string? ApplicationUserId { get; set; }
        public virtual ApplicationUser? ApplicationUser { get; set; }

        // Propriedades calculadas
        [NotMapped]
        [Display(Name = "Tamanho Formatado")]
        public string TamanhoFormatado
        {
            get
            {
                string[] tamanhos = { "B", "KB", "MB", "GB" };
                double len = TamanhoBytes;
                int ordem = 0;
                while (len >= 1024 && ordem < tamanhos.Length - 1)
                {
                    ordem++;
                    len = len / 1024;
                }
                return $"{len:0.##} {tamanhos[ordem]}";
            }
        }

        [NotMapped]
        [Display(Name = "Extensão")]
        public string Extensao => Path.GetExtension(NomeArquivo).ToLower();

        [NotMapped]
        [Display(Name = "É Imagem")]
        public bool EhImagem => new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" }.Contains(Extensao);

        [NotMapped]
        [Display(Name = "É PDF")]
        public bool EhPdf => Extensao == ".pdf";

        [NotMapped]
        [Display(Name = "Ícone")]
        public string IconeFont
        {
            get
            {
                if (EhImagem) return "fa-file-image";
                if (EhPdf) return "fa-file-pdf";
                return "fa-file";
            }
        }

        [NotMapped]
        [Display(Name = "Cor do Ícone")]
        public string IconeColor
        {
            get
            {
                if (EhImagem) return "text-success";
                if (EhPdf) return "text-danger";
                return "text-secondary";
            }
        }
    }

    // Enumeração para tipos de documentos
    public static class TipoDocumentoEnum
    {
        public const string CNH = "CNH";
        public const string RG = "RG";
        public const string CPF = "CPF";
        public const string ComprovanteResidencia = "Comprovante de Residência";
        public const string FotoPerfil = "Foto de Perfil";
        
        // Documentos de Veículos
        public const string CRLV = "CRLV";
        public const string NotaFiscal = "Nota Fiscal";
        public const string Seguro = "Seguro";
        public const string IPVA = "IPVA";
        public const string FotoVeiculo = "Foto do Veículo";
        
        // Documentos de Funcionários
        public const string ContratoTrabalho = "Contrato de Trabalho";
        public const string CarteiraTrabalho = "Carteira de Trabalho";
        
        // Outros
        public const string Outros = "Outros";

        public static List<string> ObterTiposCliente()
        {
            return new List<string> { CNH, RG, CPF, ComprovanteResidencia, FotoPerfil, Outros };
        }

        public static List<string> ObterTiposVeiculo()
        {
            return new List<string> { CRLV, NotaFiscal, Seguro, IPVA, FotoVeiculo, Outros };
        }

        public static List<string> ObterTiposFuncionario()
        {
            return new List<string> { CNH, RG, CPF, ComprovanteResidencia, ContratoTrabalho, CarteiraTrabalho, FotoPerfil, Outros };
        }
    }
}
