using System.ComponentModel.DataAnnotations;

namespace TrocaBateriaWebApp.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "O usuário ou e-mail é obrigatório")]
        [Display(Name = "Usuário ou E-mail")]
        public string UserNameOrEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "A senha é obrigatória")]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Lembrar-me")]
        public bool RememberMe { get; set; }
    }
}
