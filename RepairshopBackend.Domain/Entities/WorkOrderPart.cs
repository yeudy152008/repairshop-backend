namespace RepairshopBackend.Domain.Entities;

public class WorkOrderPart
{
    public int Id { get; set; }
    public int WorkOrderId { get; set; }
    public WorkOrder WorkOrder { get; set; } = null!;
    public int InventoryItemId { get; set; }
    public InventoryItem InventoryItem { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Cost { get; set; }
}