namespace RepairshopBackend.Domain.Entities;

public class PurchaseItem
{
    public int Id { get; set; }
    public int PurchaseId { get; set; }
    public Purchase Purchase { get; set; } = null!;
    public int InventoryItemId { get; set; }
    public InventoryItem InventoryItem { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal IvaRate { get; set; } = 13;
}