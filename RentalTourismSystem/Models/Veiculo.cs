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
        public string Marca { get; set; }

        [Required(ErrorMessage = "O modelo é obrigatório")]
        [StringLength(50)]
        public string Modelo { get; set; }

        [Required(ErrorMessage = "O ano é obrigatório")]
        [Range(1990, 2030, ErrorMessage = "Ano deve estar entre 1990 e 2030")]
        public int Ano { get; set; }

        [Required(ErrorMessage = "A placa é obrigatória")]
        [PlacaValidation]
        [StringLength(10)]
        public string Placa { get; set; }

        [Required(ErrorMessage = "A cor é obrigatória")]
        [StringLength(50)]
        public string Cor { get; set; }

        [Required(ErrorMessage = "O valor da diária é obrigatório")]
        [Range(0.01, 9999.99, ErrorMessage = "Valor da diária deve ser maior que zero")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal ValorDiaria { get; set; }

        [Required(ErrorMessage = "A quilometragem é obrigatória")]
        [Range(0, int.MaxValue, ErrorMessage = "Quilometragem deve ser maior ou igual a zero")]
        public int Quilometragem { get; set; }

        [Display(Name = "Data de Cadastro")]
        public DateTime DataCadastro { get; set; } = DateTime.Now;

        [Required]
        public int StatusCarroId { get; set; }
        public virtual StatusCarro? StatusCarro { get; set; }

        [Required]
        public int AgenciaId { get; set; }
        public virtual Agencia? Agencia { get; set; }

        public virtual ICollection<Locacao> Locacoes { get; set; } = new List<Locacao>();
    }
}
