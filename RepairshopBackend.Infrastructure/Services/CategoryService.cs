using Microsoft.EntityFrameworkCore;
using RepairshopBackend.Application.DTOs;
using RepairshopBackend.Application.Interfaces;
using RepairshopBackend.Domain.Entities;
using RepairshopBackend.Infrastructure.Data;

namespace RepairshopBackend.Infrastructure.Services;

public class CategoryService : ICategoryService
{
    private readonly AppDbContext _context;

    public CategoryService(AppDbContext context)
    {
        _context = context;
    }

    private static CategoryDto ToDto(InventoryCategory c) => new() { Id = c.Id, Name = c.Name };

    public async Task<List<CategoryDto>> GetAllAsync()
    {
        return await _context.InventoryCategories
            .OrderBy(c => c.Name)
            .Select(c => ToDto(c))
            .ToListAsync();
    }

    public async Task<(CategoryDto? category, string? error)> CreateAsync(SaveCategoryDto dto)
    {
        var duplicate = await _context.InventoryCategories.AnyAsync(c => c.Name == dto.Name);
        if (duplicate)
        {
            return (null, "Ya existe una categoría con ese nombre.");
        }

        var category = new InventoryCategory { Name = dto.Name };
        _context.InventoryCategories.Add(category);
        await _context.SaveChangesAsync();

        return (ToDto(category), null);
    }

    public async Task<(CategoryDto? category, string? error)> UpdateAsync(int id, SaveCategoryDto dto)
    {
        var category = await _context.InventoryCategories.FindAsync(id);
        if (category is null)
        {
            return (null, "Categoría no encontrada.");
        }

        var duplicate = await _context.InventoryCategories.AnyAsync(c => c.Name == dto.Name && c.Id != id);
        if (duplicate)
        {
            return (null, "Ya existe una categoría con ese nombre.");
        }

        category.Name = dto.Name;
        await _context.SaveChangesAsync();

        return (ToDto(category), null);
    }

    public async Task<(bool success, string? error)> DeleteAsync(int id)
    {
        var category = await _context.InventoryCategories.FindAsync(id);
        if (category is null)
        {
            return (false, "Categoría no encontrada.");
        }

        var hasItems = await _context.InventoryItems.AnyAsync(i => i.CategoryId == id);
        if (hasItems)
        {
            return (false, "No se puede eliminar: hay repuestos asignados a esta categoría.");
        }

        _context.InventoryCategories.Remove(category);
        await _context.SaveChangesAsync();
        return (true, null);
    }
}