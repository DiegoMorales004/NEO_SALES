namespace NEO_SALES.CORE.Models.Dtos;

public class CustomerCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string? Nit { get; set; }
    public string Email { get; set; } = string.Empty;
}
