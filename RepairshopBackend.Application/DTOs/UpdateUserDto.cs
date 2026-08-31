namespace RepairshopBackend.Application.DTOs;

public class UpdateUserDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool Active { get; set; }
}