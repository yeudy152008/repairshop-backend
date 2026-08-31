namespace RepairshopBackend.Domain.Entities;

public class WorkOrder
{
    public int Id { get; set; }
    public int VehicleId { get; set; }
    public Vehicle Vehicle { get; set; } = null!;
    public string VehiclePlate { get; set; } = string.Empty;
    public string VehicleModel { get; set; } = string.Empty;
    public string Technician { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal LaborCost { get; set; }
    public decimal HoursSpent { get; set; }
    public string Status { get; set; } = "Pendiente";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public List<WorkOrderPart> Parts { get; set; } = new();
}