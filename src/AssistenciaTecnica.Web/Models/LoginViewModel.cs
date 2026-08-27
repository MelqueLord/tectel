using System.ComponentModel.DataAnnotations;

namespace AssistenciaTecnica.Web.Models;

public class LoginViewModel
{
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    [Display(Name = "E-mail")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [DataType(DataType.Password)]
    [Display(Name = "Senha")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Lembrar-me")]
    public bool RememberMe { get; set; }
}
