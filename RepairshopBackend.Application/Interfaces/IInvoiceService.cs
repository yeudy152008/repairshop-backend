using RepairshopBackend.Application.DTOs;

namespace RepairshopBackend.Application.Interfaces;

public interface IInvoiceService
{
    Task<List<InvoiceDto>> GetAllAsync();
    Task<(InvoiceDto? invoice, string? error)> CreateDirectAsync(CreateDirectInvoiceDto dto);
    Task<(InvoiceDto? invoice, string? error)> CreateFromWorkOrderAsync(CreateInvoiceFromOrderDto dto);
}