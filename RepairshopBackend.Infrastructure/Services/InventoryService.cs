using Microsoft.EntityFrameworkCore;
using RepairshopBackend.Application.DTOs;
using RepairshopBackend.Application.Interfaces;
using RepairshopBackend.Domain.Entities;
using RepairshopBackend.Infrastructure.Data;

namespace RepairshopBackend.Infrastructure.Services;

public class InventoryService : IInventoryService
{
    private readonly AppDbContext _context;

    public InventoryService(AppDbContext context)
    {
        _context = context;
    }

    private static InventoryItemDto ToDto(InventoryItem i) => new()
    {
        Id = i.Id,
        Name = i.Name,
        Sku = i.Sku,
        CategoryId = i.CategoryId,
        CategoryName = i.Category?.Name ?? string.Empty,
        Quantity = i.Quantity,
        MinStock = i.MinStock,
        UnitCost = i.UnitCost,
        MarginPercent = i.MarginPercent,
        SalePrice = i.SalePrice,
        IvaRate = i.IvaRate,
        Active = i.Active,
    };

    public async Task<List<InventoryItemDto>> GetAllAsync()
    {
        var items = await _context.InventoryItems
            .Include(i => i.Category)
            .OrderBy(i => i.Name)
            .ToListAsync();

        return items.Select(ToDto).ToList();
    }

    public async Task<(InventoryItemDto? item, string? error)> CreateAsync(SaveInventoryItemDto dto)
    {
        var category = await _context.InventoryCategories.FindAsync(dto.CategoryId);
        if (category is null)
        {
            return (null, "La categoría seleccionada no existe.");
        }

        var duplicateSku = await _context.InventoryItems.AnyAsync(i => i.Sku == dto.Sku);
        if (duplicateSku)
        {
            return (null, "Ya existe un repuesto con este SKU.");
        }

        var item = new InventoryItem
        {
            Name = dto.Name,
            Sku = dto.Sku,
            CategoryId = dto.CategoryId,
            Quantity = dto.Quantity,
            MinStock = dto.MinStock,
            UnitCost = dto.UnitCost,
            MarginPercent = dto.MarginPercent,
            IvaRate = dto.IvaRate,
            Active = dto.Active,
        };
        item.SalePrice = Math.Round(item.UnitCost * (1 + item.MarginPercent / 100m), 2);

        _context.InventoryItems.Add(item);
        await _context.SaveChangesAsync();

        item.Category = category;
        return (ToDto(item), null);
    }

    public async Task<(InventoryItemDto? item, string? error)> UpdateAsync(int id, SaveInventoryItemDto dto)
    {
        var item = await _context.InventoryItems
            .Include(i => i.Category)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (item is null)
        {
            return (null, "Repuesto no encontrado.");
        }

        var category = await _context.InventoryCategories.FindAsync(dto.CategoryId);
        if (category is null)
        {
            return (null, "La categoría seleccionada no existe.");
        }

        var duplicateSku = await _context.InventoryItems.AnyAsync(i => i.Sku == dto.Sku && i.Id != id);
        if (duplicateSku)
        {
            return (null, "Ya existe un repuesto con este SKU.");
        }

        item.Name = dto.Name;
        item.Sku = dto.Sku;
        item.CategoryId = dto.CategoryId;
        item.Quantity = dto.Quantity;
        item.MinStock = dto.MinStock;
        item.UnitCost = dto.UnitCost;
        item.MarginPercent = dto.MarginPercent;
        item.IvaRate = dto.IvaRate;
        item.Active = dto.Active;
        item.SalePrice = Math.Round(item.UnitCost * (1 + item.MarginPercent / 100m), 2);

        await _context.SaveChangesAsync();

        item.Category = category;
        return (ToDto(item), null);
    }

    public async Task<(bool success, string? error)> DeleteAsync(int id)
    {
        var item = await _context.InventoryItems.FindAsync(id);
        if (item is null)
        {
            return (false, "Repuesto no encontrado.");
        }

        var inUse = await _context.WorkOrderParts.AnyAsync(p => p.InventoryItemId == id);
        if (inUse)
        {
            return (false, "No se puede eliminar: este repuesto ha sido utilizado en órdenes de trabajo.");
        }

        _context.InventoryItems.Remove(item);
        await _context.SaveChangesAsync();
        return (true, null);
    }
}