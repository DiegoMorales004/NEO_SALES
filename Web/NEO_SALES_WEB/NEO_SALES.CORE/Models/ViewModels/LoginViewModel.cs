using System.ComponentModel.DataAnnotations;

namespace NEO_SALES.CORE.Models.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "El usuario es obligatorio")]
    [Display(Name = "Usuario")]
    public string User { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Password { get; set; } = string.Empty;

    public string? DefaultUserHint { get; set; }
    public string? DefaultPasswordHint { get; set; }
}
