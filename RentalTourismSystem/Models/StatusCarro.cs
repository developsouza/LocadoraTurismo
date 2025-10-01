using System.ComponentModel.DataAnnotations;

namespace RentalTourismSystem.Models
{
    public class StatusCarro
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; }

        public virtual ICollection<Veiculo> Veiculos { get; set; } = new List<Veiculo>();
    }
}