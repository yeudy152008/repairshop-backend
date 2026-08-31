namespace RepairshopBackend.Domain.Entities;

public class InvoiceItem
{
    public int Id { get; set; }
    public int InvoiceId { get; set; }
    public Invoice Invoice { get; set; } = null!;
    public int InventoryItemId { get; set; }
    public InventoryItem InventoryItem { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal IvaRate { get; set; } = 13;
    public bool Exonerado { get; set; }
}