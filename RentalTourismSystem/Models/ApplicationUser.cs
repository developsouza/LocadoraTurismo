using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace RentalTourismSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string NomeCompleto { get; set; } = string.Empty;

        [StringLength(14)]
        public string? Cpf { get; set; }

        public DateTime DataCadastro { get; set; } = DateTime.Now;

        public bool Ativo { get; set; } = true;

        [StringLength(100)]
        public string? Cargo { get; set; }

        public int? AgenciaId { get; set; }
        public virtual Agencia? Agencia { get; set; }

        // Navegação para funcionário (se existir)
        public int? FuncionarioId { get; set; }
        public virtual Funcionario? Funcionario { get; set; }
    }
}