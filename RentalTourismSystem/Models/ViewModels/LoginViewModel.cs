using System.ComponentModel.DataAnnotations;

namespace RentalTourismSystem.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Senha é obrigatória")]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Lembrar-me")]
        public bool RememberMe { get; set; }

        public string? ReturnUrl { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Nome completo é obrigatório")]
        [StringLength(100)]
        [Display(Name = "Nome Completo")]
        public string NomeCompleto { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Senha é obrigatória")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter pelo menos 6 caracteres")]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirmação de senha é obrigatória")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "As senhas não coincidem")]
        [Display(Name = "Confirmar Senha")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Display(Name = "CPF")]
        [StringLength(14)]
        public string? Cpf { get; set; }

        [Display(Name = "Cargo")]
        [StringLength(100)]
        public string? Cargo { get; set; }

        [Display(Name = "Agência")]
        public int? AgenciaId { get; set; }
    }

    public class EditUserViewModel
    {
        [Required]
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nome completo é obrigatório")]
        [StringLength(100)]
        [Display(Name = "Nome Completo")]
        public string NomeCompleto { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "CPF")]
        [StringLength(14)]
        public string? Cpf { get; set; }

        [Display(Name = "Cargo")]
        [StringLength(100)]
        public string? Cargo { get; set; }

        [Display(Name = "Agência")]
        public int? AgenciaId { get; set; }

        [Display(Name = "Ativo")]
        public bool Ativo { get; set; }

        [Display(Name = "Permissão")]
        public string? Role { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Nova Senha (deixe em branco para manter a atual)")]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "As senhas não coincidem")]
        [Display(Name = "Confirmar Nova Senha")]
        public string? ConfirmPassword { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Senha atual é obrigatória")]
        [DataType(DataType.Password)]
        [Display(Name = "Senha Atual")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nova senha é obrigatória")]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Nova Senha")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirmação de senha é obrigatória")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "As senhas não coincidem")]
        [Display(Name = "Confirmar Nova Senha")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}