namespace NEO_SALES.CORE.Models.Dtos.Sale;

public class ConfirmSaleResultDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<ConfirmSaleLineDto> Lines { get; set; } = [];
}

public class ConfirmSaleLineDto
{
    public Guid IdProduct { get; set; }
    public string Name { get; set; } = string.Empty;
    public int QuantityRequested { get; set; }
    public int? StockAvailable { get; set; }
    public int? QuantityMissing { get; set; }
    public int? StockRemaining { get; set; }
}
