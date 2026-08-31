namespace RepairshopBackend.Application.DTOs;

public class CreateMovementLogDto
{
    public string Type { get; set; } = string.Empty;
    public string Module { get; set; } = string.Empty;
    public string Detail { get; set; } = string.Empty;
}