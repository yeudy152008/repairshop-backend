namespace RepairshopBackend.Application.DTOs;

public class AccessLogDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public DateTime LoginAt { get; set; }
    public DateTime? LogoutAt { get; set; }
}