using RepairshopBackend.Application.DTOs;

namespace RepairshopBackend.Application.Interfaces;

public interface ICategoryService
{
    Task<List<CategoryDto>> GetAllAsync();
    Task<(CategoryDto? category, string? error)> CreateAsync(SaveCategoryDto dto);
    Task<(CategoryDto? category, string? error)> UpdateAsync(int id, SaveCategoryDto dto);
    Task<(bool success, string? error)> DeleteAsync(int id);
}