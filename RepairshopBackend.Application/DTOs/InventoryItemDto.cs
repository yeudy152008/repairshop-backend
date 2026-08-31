namespace RepairshopBackend.Application.DTOs;

public class InventoryItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public int MinStock { get; set; }
    public decimal UnitCost { get; set; }
    public decimal MarginPercent { get; set; }
    public decimal SalePrice { get; set; }
    public decimal IvaRate { get; set; }
    public bool Active { get; set; }
}