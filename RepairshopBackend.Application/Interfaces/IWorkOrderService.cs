using RepairshopBackend.Application.DTOs;

namespace RepairshopBackend.Application.Interfaces;

public interface IWorkOrderService
{
    Task<List<WorkOrderDto>> GetAllAsync();
    Task<(WorkOrderDto? order, string? error)> CreateAsync(CreateWorkOrderDto dto);
    Task<(WorkOrderDto? order, string? error)> UpdateAsync(UpdateWorkOrderDto dto);
    Task<(bool success, string? error)> DeleteAsync(int id);
}