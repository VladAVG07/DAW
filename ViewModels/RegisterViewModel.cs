using System.ComponentModel.DataAnnotations;

namespace Lab06.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Username-ul este obligatoriu")]
    [MinLength(3, ErrorMessage = "Username-ul trebuie sa aiba minim 3 caractere")]
    [Display(Name = "Username")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email-ul este obligatoriu")]
    [EmailAddress(ErrorMessage = "Format email invalid")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Numele complet este obligatoriu")]
    [MinLength(3, ErrorMessage = "Numele trebuie sa aiba minim 3 caractere")]
    [Display(Name = "Nume complet")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Parola este obligatorie")]
    [DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = "Parola trebuie sa aiba minim 6 caractere")]
    [Display(Name = "Parola")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirmarea parolei este obligatorie")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Parolele nu coincid")]
    [Display(Name = "Confirmare parola")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
