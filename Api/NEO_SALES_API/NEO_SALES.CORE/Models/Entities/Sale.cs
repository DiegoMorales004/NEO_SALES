namespace NEO_SALES.CORE.Models.Entities;

public class Sale
{
    public Guid Id { get; set; }
    public Guid IdCustomer { get; set; }
    public int StatusId { get; set; }
    public DateTime Date { get; set; }
}
