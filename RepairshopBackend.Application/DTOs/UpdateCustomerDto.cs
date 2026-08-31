namespace RepairshopBackend.Application.DTOs;

public class UpdateCustomerDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string IdNumber { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? ExemptionLetterNumber { get; set; }
    public bool Active { get; set; }
}