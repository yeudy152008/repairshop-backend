namespace RepairshopBackend.Domain.Entities;

public class Purchase
{
    public int Id { get; set; }
    public int SupplierId { get; set; }
    public Supplier Supplier { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public List<PurchaseItem> Items { get; set; } = new();
}