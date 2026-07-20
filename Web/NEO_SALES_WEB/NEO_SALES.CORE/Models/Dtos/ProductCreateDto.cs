namespace NEO_SALES.CORE.Models.Dtos;

public class ProductCreateDto
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
}
