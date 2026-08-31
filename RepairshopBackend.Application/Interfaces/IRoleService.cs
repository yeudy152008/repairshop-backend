using RepairshopBackend.Application.DTOs;

namespace RepairshopBackend.Application.Interfaces;

public interface IRoleService
{
    Task<List<RoleDto>> GetAllAsync();
    Task<(RoleDto? role, string? error)> CreateAsync(SaveRoleDto dto);
    Task<(RoleDto? role, string? error)> UpdateAsync(int id, SaveRoleDto dto);
    Task<(bool success, string? error)> DeleteAsync(int id);
}