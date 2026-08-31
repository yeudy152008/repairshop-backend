namespace RepairshopBackend.Application.DTOs;

public class RoleDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Dictionary<string, bool> Permissions { get; set; } = new();
}