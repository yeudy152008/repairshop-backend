using RepairshopBackend.Application.DTOs;

namespace RepairshopBackend.Application.Interfaces;

public interface IInventoryService
{
    Task<List<InventoryItemDto>> GetAllAsync();
    Task<(InventoryItemDto? item, string? error)> CreateAsync(SaveInventoryItemDto dto);
    Task<(InventoryItemDto? item, string? error)> UpdateAsync(int id, SaveInventoryItemDto dto);
    Task<(bool success, string? error)> DeleteAsync(int id);
}