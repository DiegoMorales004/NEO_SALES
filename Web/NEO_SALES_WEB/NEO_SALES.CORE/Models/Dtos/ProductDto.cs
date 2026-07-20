namespace NEO_SALES.CORE.Models.Dtos;

public class ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public bool Status { get; set; }
    public DateTime DatetimeCreate { get; set; }
    public DateTime? DatetimeUpdate { get; set; }
}
