using System.ComponentModel.DataAnnotations;

namespace AssistenciaTecnica.Web.Models;

public class CriarUsuarioViewModel
{
    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    [Display(Name = "E-mail")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "A senha é obrigatória.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres.")]
    [DataType(DataType.Password)]
    [Display(Name = "Senha")]
    public string Senha { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirme a senha.")]
    [Compare("Senha", ErrorMessage = "As senhas não conferem.")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirmar Senha")]
    public string ConfirmarSenha { get; set; } = string.Empty;

    [Display(Name = "Administrador")]
    public bool IsAdmin { get; set; }
}

public class EditarUsuarioViewModel
{
    public string Id { get; set; } = string.Empty;

    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    [Display(Name = "E-mail")]
    public string Email { get; set; } = string.Empty;

    [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres.")]
    [DataType(DataType.Password)]
    [Display(Name = "Nova Senha (deixe vazio para não alterar)")]
    public string? NovaSenha { get; set; }

    [Display(Name = "Administrador")]
    public bool IsAdmin { get; set; }

    [Display(Name = "Ativo")]
    public bool Ativo { get; set; } = true;
}
