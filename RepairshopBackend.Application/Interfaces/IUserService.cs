using RepairshopBackend.Application.DTOs;

namespace RepairshopBackend.Application.Interfaces;

public interface IUserService
{
    Task<List<UserDto>> GetAllAsync();
    Task<(UserDto? user, string? error)> CreateAsync(CreateUserDto dto);
    Task<(UserDto? user, string? error)> UpdateAsync(UpdateUserDto dto);
    Task<bool> DeleteAsync(int id);
}