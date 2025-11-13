using System.ComponentModel.DataAnnotations;

namespace TrocaBateriaWebApp.Models
{

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "O nome de usuário é obrigatório")]
        [Display(Name = "Usuário")]
        [StringLength(50, ErrorMessage = "O usuário deve ter no máximo 50 caracteres")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "O e-mail é obrigatório")]
        [EmailAddress(ErrorMessage = "E-mail inválido")]
        [Display(Name = "E-mail")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A senha é obrigatória")]
        [StringLength(100, ErrorMessage = "A senha deve ter no mínimo {2} caracteres", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "A confirmação de senha é obrigatória")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Senha")]
        [Compare("Password", ErrorMessage = "As senhas não coincidem")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "O nome completo é obrigatório")]
        [Display(Name = "Nome Completo")]
        [StringLength(100)]
        public string NomeCompleto { get; set; } = string.Empty;

        [Display(Name = "Telefone")]
        [Phone(ErrorMessage = "Telefone inválido")]
        public string? Telefone { get; set; }

        [Required(ErrorMessage = "Selecione o tipo de conta")]
        [Display(Name = "Tipo de Conta")]
        public string TipoConta { get; set; } = "Usuario";

        [Display(Name = "CNPJ")]
        [StringLength(18, ErrorMessage = "CNPJ inválido")]
        public string? Cnpj { get; set; }

        [Display(Name = "Endereço")]
        [StringLength(200)]
        public string? Endereco { get; set; }
    }
}
