namespace RepairshopBackend.Application.DTOs;

public class InvoiceItemDto
{
    public int Id { get; set; }
    public int InventoryItemId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal LineSubtotal { get; set; }
    public decimal IvaRate { get; set; }
    public bool Exonerado { get; set; }
    public decimal LineIva { get; set; }
}