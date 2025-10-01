using System.ComponentModel.DataAnnotations;

namespace RentalTourismSystem.Models
{
    public class StatusReservaViagem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; }

        public virtual ICollection<ReservaViagem> ReservasViagens { get; set; } = new List<ReservaViagem>();
    }
}