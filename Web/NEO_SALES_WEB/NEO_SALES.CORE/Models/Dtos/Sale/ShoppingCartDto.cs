namespace NEO_SALES.CORE.Models.Dtos.Sale;

public class ShoppingCartDto
{
    public Guid IdSale { get; set; }
    public Guid IdCustomer { get; set; }
    public DateTime Date { get; set; }
    public List<ShoppingCartItemDto> Items { get; set; } = [];
    public decimal Total { get; set; }
}

public class ShoppingCartItemDto
{
    public Guid IdSaleDetail { get; set; }
    public Guid IdProduct { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Stock { get; set; }
    public int Quantity { get; set; }
    public decimal PriceUnit { get; set; }
    public decimal Subtotal { get; set; }
}
