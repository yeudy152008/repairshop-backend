namespace RepairshopBackend.Application.DTOs;

public class CreateDirectInvoiceDto
{
    public int CustomerId { get; set; }
    public List<CreateInvoiceItemDto> Items { get; set; } = new();
}