using System.ComponentModel.DataAnnotations;

namespace NEO_SALES.CORE.Models.ViewModels;

public class CustomerFormViewModel
{
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(150)]
    [Display(Name = "Nombre")]
    public string Name { get; set; } = string.Empty;

    [StringLength(30)]
    [Display(Name = "NIT")]
    public string? Nit { get; set; }

    [Required(ErrorMessage = "El correo es obligatorio")]
    [EmailAddress(ErrorMessage = "El correo no tiene un formato válido")]
    [Display(Name = "Correo electrónico")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Activo")]
    public bool Status { get; set; } = true;
}
