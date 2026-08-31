namespace RepairshopBackend.Domain.Entities;

public class InventoryItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public InventoryCategory Category { get; set; } = null!;
    public int Quantity { get; set; }
    public int MinStock { get; set; }
    public decimal UnitCost { get; set; }
    public decimal MarginPercent { get; set; } = 30;
    public decimal SalePrice { get; set; }
    public decimal IvaRate { get; set; } = 13;
    public bool Active { get; set; } = true;
}