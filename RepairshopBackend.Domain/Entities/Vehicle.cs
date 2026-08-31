namespace RepairshopBackend.Domain.Entities;

public class Vehicle
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Plate { get; set; } = string.Empty;
}