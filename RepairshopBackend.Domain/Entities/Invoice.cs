namespace RepairshopBackend.Domain.Entities;

public class Invoice
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    public int? WorkOrderId { get; set; }
    public WorkOrder? WorkOrder { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public decimal LaborCost { get; set; }
    public List<InvoiceItem> Items { get; set; } = new();
}