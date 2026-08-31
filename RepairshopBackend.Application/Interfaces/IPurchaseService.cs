using RepairshopBackend.Application.DTOs;

namespace RepairshopBackend.Application.Interfaces;

public interface IPurchaseService
{
    Task<List<PurchaseDto>> GetAllAsync();
    Task<(PurchaseDto? purchase, string? error)> CreateAsync(CreatePurchaseDto dto);
}