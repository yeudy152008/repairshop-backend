namespace RepairshopBackend.Application.DTOs;

public class SaveSupplierDto
{
    public string Name { get; set; } = string.Empty;
    public string ContactName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool Active { get; set; } = true;
}