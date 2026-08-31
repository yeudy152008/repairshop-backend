using Microsoft.EntityFrameworkCore;
using RepairshopBackend.Application.DTOs;
using RepairshopBackend.Application.Interfaces;
using RepairshopBackend.Domain.Entities;
using RepairshopBackend.Infrastructure.Data;

namespace RepairshopBackend.Infrastructure.Services;

public class PurchaseService : IPurchaseService
{
    private readonly AppDbContext _context;

    public PurchaseService(AppDbContext context)
    {
        _context = context;
    }

    private static PurchaseDto ToDto(Purchase p)
    {
        var items = p.Items.Select(i =>
        {
            var lineSubtotal = i.Quantity * i.UnitCost * (1 - i.DiscountPercent / 100m);
            var lineIva = Math.Round(lineSubtotal * (i.IvaRate / 100m), 2);

            return new PurchaseItemDto
            {
                Id = i.Id,
                InventoryItemId = i.InventoryItemId,
                Name = i.Name,
                Quantity = i.Quantity,
                UnitCost = i.UnitCost,
                DiscountPercent = i.DiscountPercent,
                LineSubtotal = Math.Round(lineSubtotal, 2),
                IvaRate = i.IvaRate,
                LineIva = lineIva,
            };
        }).ToList();

        var subtotal = items.Sum(i => i.LineSubtotal);
        var ivaAmount = items.Sum(i => i.LineIva);

        return new PurchaseDto
        {
            Id = p.Id,
            SupplierId = p.SupplierId,
            SupplierName = p.Supplier?.Name ?? string.Empty,
            CreatedAt = p.CreatedAt,
            Items = items,
            Subtotal = Math.Round(subtotal, 2),
            IvaAmount = Math.Round(ivaAmount, 2),
            Total = Math.Round(subtotal + ivaAmount, 2),
        };
    }

    public async Task<List<PurchaseDto>> GetAllAsync()
    {
        var purchases = await _context.Purchases
            .Include(p => p.Supplier)
            .Include(p => p.Items)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return purchases.Select(ToDto).ToList();
    }

    public async Task<(PurchaseDto? purchase, string? error)> CreateAsync(CreatePurchaseDto dto)
    {
        if (dto.Items.Count == 0)
        {
            return (null, "Debe agregar al menos un repuesto a la compra.");
        }

        var supplier = await _context.Suppliers.FindAsync(dto.SupplierId);
        if (supplier is null)
        {
            return (null, "El proveedor seleccionado no existe.");
        }

        var purchase = new Purchase
        {
            SupplierId = supplier.Id,
            CreatedAt = DateTime.UtcNow,
        };

        foreach (var requested in dto.Items)
        {
            if (requested.Quantity <= 0)
            {
                return (null, "La cantidad de cada repuesto debe ser mayor a cero.");
            }

            if (requested.DiscountPercent < 0 || requested.DiscountPercent > 100)
            {
                return (null, "El descuento debe estar entre 0 y 100%.");
            }

            var item = await _context.InventoryItems.FindAsync(requested.InventoryItemId);
            if (item is null)
            {
                return (null, "Uno de los repuestos seleccionados ya no existe en el inventario.");
            }

            // El costo real que actualiza el Inventario es el costo neto pagado,
            // ya con el descuento del proveedor aplicado (sin IVA, ya que el IVA
            // no forma parte del costo base para calcular margen de ganancia).
            var effectiveUnitCost = Math.Round(requested.UnitCost * (1 - requested.DiscountPercent / 100m), 2);

            item.Quantity += requested.Quantity;
            item.UnitCost = effectiveUnitCost;
            item.SalePrice = Math.Round(item.UnitCost * (1 + item.MarginPercent / 100m), 2);

            purchase.Items.Add(new PurchaseItem
            {
                InventoryItemId = item.Id,
                Name = item.Name,
                Quantity = requested.Quantity,
                UnitCost = requested.UnitCost,
                DiscountPercent = requested.DiscountPercent,
                IvaRate = item.IvaRate,
            });
        }

        _context.Purchases.Add(purchase);
        await _context.SaveChangesAsync();

        purchase.Supplier = supplier;
        return (ToDto(purchase), null);
    }
}
