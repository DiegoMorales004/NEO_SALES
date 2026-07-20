namespace NEO_SALES.CORE.Models.Dtos;

public class ProductEditDto
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public bool Status { get; set; }
}
