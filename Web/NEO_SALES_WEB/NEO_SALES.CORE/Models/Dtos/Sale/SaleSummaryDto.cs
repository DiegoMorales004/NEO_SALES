namespace NEO_SALES.CORE.Models.Dtos.Sale;

public class SaleSummaryDto
{
    public Guid Id { get; set; }
    public Guid IdCustomer { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public int StatusId { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
}
