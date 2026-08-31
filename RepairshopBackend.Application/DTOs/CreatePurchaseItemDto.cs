namespace RepairshopBackend.Application.DTOs;

public class CreatePurchaseItemDto
{
    public int InventoryItemId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal DiscountPercent { get; set; }
}
