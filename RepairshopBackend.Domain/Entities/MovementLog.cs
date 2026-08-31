namespace RepairshopBackend.Domain.Entities;

public class MovementLog
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Module { get; set; } = string.Empty;
    public string Detail { get; set; } = string.Empty;
}