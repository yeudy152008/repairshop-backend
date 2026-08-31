namespace RepairshopBackend.Application.DTOs;

public class WorkOrderDto
{
    public int Id { get; set; }
    public int VehicleId { get; set; }
    public string VehiclePlate { get; set; } = string.Empty;
    public string VehicleModel { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string Technician { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal LaborCost { get; set; }
    public decimal HoursSpent { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<WorkOrderPartDto> Parts { get; set; } = new();
}