namespace RepairshopBackend.Application.DTOs;

public class WorkOrderPartDto
{
    public int Id { get; set; }
    public int InventoryItemId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Cost { get; set; }
}