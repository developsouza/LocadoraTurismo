using RentalTourismSystem.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentalTourismSystem.Models
{
    public partial class Veiculo
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "A marca é obrigatória")]
        [StringLength(50)]
        [Display(Name = "Marca")]
        public string Marca { get; set; }

        [Required(ErrorMessage = "O modelo é obrigatório")]
        [StringLength(50)]
        [Display(Name = "Modelo")]
        public string Modelo { get; set; }

        [Required(ErrorMessage = "O ano é obrigatório")]
        [Range(1990, 2030, ErrorMessage = "Ano deve estar entre 1990 e 2030")]
        [Display(Name = "Ano")]
        public int Ano { get; set; }

        [Required(ErrorMessage = "A placa é obrigatória")]
        [PlacaValidation]
        [StringLength(10)]
        [Display(Name = "Placa")]
        public string Placa { get; set; }

        [Required(ErrorMessage = "A cor é obrigatória")]
        [StringLength(50)]
        [Display(Name = "Cor")]
        public string Cor { get; set; }

        // ✅ CORRIGIDO: Required mantido (campos agora estão no Bind do Controller)
        [Required(ErrorMessage = "O tipo de combustível é obrigatório")]
        [StringLength(30)]
        [Display(Name = "Combustível")]
        public string Combustivel { get; set; }

        // ✅ CORRIGIDO: Required mantido (campos agora estão no Bind do Controller)
        [Required(ErrorMessage = "O tipo de câmbio é obrigatório")]
        [StringLength(30)]
        [Display(Name = "Câmbio")]
        public string Cambio { get; set; }

        [Required(ErrorMessage = "O valor da diária é obrigatório")]
        [Column(TypeName = "decimal(10,2)")]
        [Display(Name = "Valor da Diária")]
        public decimal ValorDiaria { get; set; }

        [Required(ErrorMessage = "A quilometragem é obrigatória")]
        [Range(0, int.MaxValue, ErrorMessage = "Quilometragem deve ser maior ou igual a zero")]
        [Display(Name = "Quilometragem")]
        public int Quilometragem { get; set; }

        [Display(Name = "Data de Cadastro")]
        public DateTime DataCadastro { get; set; } = DateTime.Now;

        [Required]
        [Display(Name = "Status")]
        public int StatusCarroId { get; set; }
        public virtual StatusCarro? StatusCarro { get; set; }

        [Required]
        [Display(Name = "Agência")]
        public int AgenciaId { get; set; }
        public virtual Agencia? Agencia { get; set; }

        public virtual ICollection<Locacao> Locacoes { get; set; } = new List<Locacao>();
    }
}