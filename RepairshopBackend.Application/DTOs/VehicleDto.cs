namespace RepairshopBackend.Application.DTOs;

public class VehicleDto
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Plate { get; set; } = string.Empty;
}