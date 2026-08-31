namespace RepairshopBackend.Application.DTOs;

public class SaveVehicleDto
{
    public int CustomerId { get; set; }
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Plate { get; set; } = string.Empty;
}