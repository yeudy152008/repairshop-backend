namespace RepairshopBackend.Application.DTOs;

public class InvoiceDto
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public int? WorkOrderId { get; set; }
    public DateTime CreatedAt { get; set; }
    public decimal LaborCost { get; set; }
    public List<InvoiceItemDto> Items { get; set; } = new();
    public decimal Subtotal { get; set; }
    public decimal IvaAmount { get; set; }
    public decimal Total { get; set; }
}