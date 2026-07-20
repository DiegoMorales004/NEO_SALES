namespace NEO_SALES.CORE.Models.Entities;

public class SaleDetail
{
    public Guid Id { get; set; }
    public Guid IdSale { get; set; }
    public Guid IdProduct { get; set; }
    public int Quantity { get; set; }
    public decimal PriceUnit { get; set; }
}
