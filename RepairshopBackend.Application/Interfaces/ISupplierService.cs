using RepairshopBackend.Application.DTOs;

namespace RepairshopBackend.Application.Interfaces;

public interface ISupplierService
{
    Task<List<SupplierDto>> GetAllAsync();
    Task<(SupplierDto? supplier, string? error)> CreateAsync(SaveSupplierDto dto);
    Task<(SupplierDto? supplier, string? error)> UpdateAsync(int id, SaveSupplierDto dto);
    Task<(bool success, string? error)> DeleteAsync(int id);
}