namespace RepairshopBackend.Application.DTOs;

public class UpdateWorkOrderDto
{
    public int Id { get; set; }
    public string Technician { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal LaborCost { get; set; }
    public decimal HoursSpent { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<CreatePartDto> Parts { get; set; } = new();
}