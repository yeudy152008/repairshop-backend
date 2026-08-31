namespace RepairshopBackend.Application.DTOs;

public class CreateInvoiceItemDto
{
    public int InventoryItemId { get; set; }
    public int Quantity { get; set; }
    public decimal DiscountPercent { get; set; }
    public bool Exonerado { get; set; }
}