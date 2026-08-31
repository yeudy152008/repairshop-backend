namespace RepairshopBackend.Application.DTOs;

public class SaveRoleDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Dictionary<string, bool> Permissions { get; set; } = new();
}