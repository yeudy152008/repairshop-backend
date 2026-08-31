namespace RepairshopBackend.Domain.Entities;

public class Customer
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string IdNumber { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? ExemptionLetterNumber { get; set; }
    public bool Active { get; set; } = true;
    public List<Vehicle> Vehicles { get; set; } = new();
}