namespace RepairshopBackend.Domain.Entities;

public class Permission
{
    public int Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Module { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool Danger { get; set; }
}