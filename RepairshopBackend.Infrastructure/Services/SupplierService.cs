using Microsoft.EntityFrameworkCore;
using RepairshopBackend.Application.DTOs;
using RepairshopBackend.Application.Interfaces;
using RepairshopBackend.Domain.Entities;
using RepairshopBackend.Infrastructure.Data;

namespace RepairshopBackend.Infrastructure.Services;

public class SupplierService : ISupplierService
{
    private readonly AppDbContext _context;

    public SupplierService(AppDbContext context)
    {
        _context = context;
    }

    private static SupplierDto ToDto(Supplier s) => new()
    {
        Id = s.Id,
        Name = s.Name,
        ContactName = s.ContactName,
        Phone = s.Phone,
        Email = s.Email,
        Active = s.Active,
    };

    public async Task<List<SupplierDto>> GetAllAsync()
    {
        return await _context.Suppliers
            .OrderBy(s => s.Name)
            .Select(s => ToDto(s))
            .ToListAsync();
    }

    public async Task<(SupplierDto? supplier, string? error)> CreateAsync(SaveSupplierDto dto)
    {
        var duplicate = await _context.Suppliers.AnyAsync(s => s.Name == dto.Name);
        if (duplicate)
        {
            return (null, "Ya existe un proveedor con ese nombre.");
        }

        var supplier = new Supplier
        {
            Name = dto.Name,
            ContactName = dto.ContactName,
            Phone = dto.Phone,
            Email = dto.Email,
            Active = dto.Active,
        };

        _context.Suppliers.Add(supplier);
        await _context.SaveChangesAsync();

        return (ToDto(supplier), null);
    }

    public async Task<(SupplierDto? supplier, string? error)> UpdateAsync(int id, SaveSupplierDto dto)
    {
        var supplier = await _context.Suppliers.FindAsync(id);
        if (supplier is null)
        {
            return (null, "Proveedor no encontrado.");
        }

        var duplicate = await _context.Suppliers.AnyAsync(s => s.Name == dto.Name && s.Id != id);
        if (duplicate)
        {
            return (null, "Ya existe un proveedor con ese nombre.");
        }

        supplier.Name = dto.Name;
        supplier.ContactName = dto.ContactName;
        supplier.Phone = dto.Phone;
        supplier.Email = dto.Email;
        supplier.Active = dto.Active;

        await _context.SaveChangesAsync();

        return (ToDto(supplier), null);
    }

    public async Task<(bool success, string? error)> DeleteAsync(int id)
    {
        var supplier = await _context.Suppliers.FindAsync(id);
        if (supplier is null)
        {
            return (false, "Proveedor no encontrado.");
        }

        var hasPurchases = await _context.Purchases.AnyAsync(p => p.SupplierId == id);
        if (hasPurchases)
        {
            return (false, "No se puede eliminar: este proveedor tiene compras registradas.");
        }

        _context.Suppliers.Remove(supplier);
        await _context.SaveChangesAsync();
        return (true, null);
    }
}