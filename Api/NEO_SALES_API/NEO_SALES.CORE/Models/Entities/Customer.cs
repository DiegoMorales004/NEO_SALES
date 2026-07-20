namespace NEO_SALES.CORE.Models.Entities;

public class Customer
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Nit { get; set; }
    public string Email { get; set; } = string.Empty;
    public bool Status { get; set; }
    public DateTime DatetimeCreate { get; set; }
    public DateTime? DatetimeUpdate { get; set; }
    public string UserCreate { get; set; } = string.Empty;
    public string? UserUpdate { get; set; }
}
