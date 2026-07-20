namespace NEO_SALES.CORE.Models.Dtos;

public class SaleDto
{
    public Guid Id { get; set; }
    public Guid IdCustomer { get; set; }
    public int StatusId { get; set; }
    public DateTime Date { get; set; }
}
