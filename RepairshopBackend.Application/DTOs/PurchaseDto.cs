namespace RepairshopBackend.Application.DTOs;

public class PurchaseDto
{
    public int Id { get; set; }
    public int SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<PurchaseItemDto> Items { get; set; } = new();
    public decimal Subtotal { get; set; }
    public decimal IvaAmount { get; set; }
    public decimal Total { get; set; }
}
