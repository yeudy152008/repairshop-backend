namespace RepairshopBackend.Domain.Entities;

public class AccessLog
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public DateTime LoginAt { get; set; }
    public DateTime? LogoutAt { get; set; }
}