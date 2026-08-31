using RepairshopBackend.Application.DTOs;

namespace RepairshopBackend.Application.Interfaces;

public interface IVehicleService
{
    Task<List<VehicleDto>> GetAllAsync();
    Task<(VehicleDto? vehicle, string? error)> CreateAsync(SaveVehicleDto dto);
    Task<(VehicleDto? vehicle, string? error)> UpdateAsync(int id, SaveVehicleDto dto);
    Task<(bool success, string? error)> DeleteAsync(int id);
}