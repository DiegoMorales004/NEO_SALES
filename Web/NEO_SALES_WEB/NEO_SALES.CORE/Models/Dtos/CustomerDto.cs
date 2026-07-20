namespace NEO_SALES.CORE.Models.Dtos;

public class CustomerDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Nit { get; set; }
    public string Email { get; set; } = string.Empty;
    public bool Status { get; set; }
    public DateTime DatetimeCreate { get; set; }
    public DateTime? DatetimeUpdate { get; set; }
}
