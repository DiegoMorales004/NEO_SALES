namespace NEO_SALES.CORE.Models.Dtos;

public class SaleDetailCreateDto
{
    public Guid IdSale { get; set; }
    public Guid IdProduct { get; set; }
    public int Quantity { get; set; }
}
