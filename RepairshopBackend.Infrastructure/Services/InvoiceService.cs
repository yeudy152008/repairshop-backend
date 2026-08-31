using Microsoft.EntityFrameworkCore;
using RepairshopBackend.Application.DTOs;
using RepairshopBackend.Application.Interfaces;
using RepairshopBackend.Domain.Entities;
using RepairshopBackend.Infrastructure.Data;

namespace RepairshopBackend.Infrastructure.Services;

public class InvoiceService : IInvoiceService
{
    private const decimal LaborIvaRate = 0.13m;

    private readonly AppDbContext _context;

    public InvoiceService(AppDbContext context)
    {
        _context = context;
    }

    private static InvoiceDto ToDto(Invoice inv)
    {
        var items = inv.Items.Select(it =>
        {
            var lineSubtotal = it.Quantity * it.UnitPrice * (1 - it.DiscountPercent / 100m);
            var lineIva = it.Exonerado ? 0m : Math.Round(lineSubtotal * (it.IvaRate / 100m), 2);

            return new InvoiceItemDto
            {
                Id = it.Id,
                InventoryItemId = it.InventoryItemId,
                Name = it.Name,
                Quantity = it.Quantity,
                UnitPrice = it.UnitPrice,
                DiscountPercent = it.DiscountPercent,
                LineSubtotal = Math.Round(lineSubtotal, 2),
                IvaRate = it.IvaRate,
                Exonerado = it.Exonerado,
                LineIva = lineIva,
            };
        }).ToList();

        var itemsSubtotal = items.Sum(i => i.LineSubtotal);
        var itemsIva = items.Sum(i => i.LineIva);
        var laborIva = Math.Round(inv.LaborCost * LaborIvaRate, 2);

        var subtotal = itemsSubtotal + inv.LaborCost;
        var ivaAmount = itemsIva + laborIva;

        return new InvoiceDto
        {
            Id = inv.Id,
            CustomerId = inv.CustomerId,
            CustomerName = inv.Customer?.FullName ?? string.Empty,
            WorkOrderId = inv.WorkOrderId,
            CreatedAt = inv.CreatedAt,
            LaborCost = inv.LaborCost,
            Items = items,
            Subtotal = Math.Round(subtotal, 2),
            IvaAmount = Math.Round(ivaAmount, 2),
            Total = Math.Round(subtotal + ivaAmount, 2),
        };
    }

    public async Task<List<InvoiceDto>> GetAllAsync()
    {
        var invoices = await _context.Invoices
            .Include(i => i.Customer)
            .Include(i => i.Items)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();

        return invoices.Select(ToDto).ToList();
    }

    public async Task<(InvoiceDto? invoice, string? error)> CreateDirectAsync(CreateDirectInvoiceDto dto)
    {
        if (dto.Items.Count == 0)
        {
            return (null, "Debe agregar al menos un artículo a la factura.");
        }

        var customer = await _context.Customers.FindAsync(dto.CustomerId);
        if (customer is null)
        {
            return (null, "El cliente seleccionado no existe.");
        }

        var invoice = new Invoice
        {
            CustomerId = customer.Id,
            WorkOrderId = null,
            CreatedAt = DateTime.UtcNow,
            LaborCost = 0,
        };

        var (items, error) = await BuildItemsAndDiscountStockAsync(dto.Items);
        if (error is not null)
        {
            return (null, error);
        }

        invoice.Items = items;

        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync();

        invoice.Customer = customer;
        return (ToDto(invoice), null);
    }

    public async Task<(InvoiceDto? invoice, string? error)> CreateFromWorkOrderAsync(CreateInvoiceFromOrderDto dto)
    {
        var order = await _context.WorkOrders
            .Include(o => o.Parts)
            .Include(o => o.Vehicle)
            .ThenInclude(v => v.Customer)
            .FirstOrDefaultAsync(o => o.Id == dto.WorkOrderId);

        if (order is null)
        {
            return (null, "La orden de trabajo no existe.");
        }

        if (order.Status != "Finalizada")
        {
            return (null, "Solo se pueden facturar órdenes de trabajo finalizadas.");
        }

        var alreadyInvoiced = await _context.Invoices.AnyAsync(i => i.WorkOrderId == order.Id);
        if (alreadyInvoiced)
        {
            return (null, "Esta orden de trabajo ya fue facturada.");
        }

        var invoice = new Invoice
        {
            CustomerId = order.Vehicle.CustomerId,
            WorkOrderId = order.Id,
            CreatedAt = DateTime.UtcNow,
            LaborCost = order.LaborCost,
        };

        // Los repuestos se facturan a su precio de venta ACTUAL (no al costo histórico
        // guardado en la orden), con la tasa de IVA actual del repuesto, sin exoneración
        // (la exoneración solo aplica a Venta Directa por ahora).
        foreach (var part in order.Parts)
        {
            var item = await _context.InventoryItems.FindAsync(part.InventoryItemId);
            invoice.Items.Add(new InvoiceItem
            {
                InventoryItemId = part.InventoryItemId,
                Name = part.Name,
                Quantity = part.Quantity,
                UnitPrice = item?.SalePrice ?? part.Cost,
                DiscountPercent = 0,
                IvaRate = item?.IvaRate ?? 13,
                Exonerado = false,
            });
        }

        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync();

        invoice.Customer = order.Vehicle.Customer;
        return (ToDto(invoice), null);
    }

    private async Task<(List<InvoiceItem> items, string? error)> BuildItemsAndDiscountStockAsync(List<CreateInvoiceItemDto> requested)
    {
        var result = new List<InvoiceItem>();

        foreach (var r in requested)
        {
            if (r.Quantity <= 0)
            {
                return (result, "La cantidad de cada artículo debe ser mayor a cero.");
            }

            if (r.DiscountPercent < 0 || r.DiscountPercent > 100)
            {
                return (result, "El descuento debe estar entre 0 y 100%.");
            }

            var item = await _context.InventoryItems.FindAsync(r.InventoryItemId);
            if (item is null)
            {
                return (result, "Uno de los artículos seleccionados ya no existe en el inventario.");
            }

            if (item.Quantity < r.Quantity)
            {
                return (result, $"No hay suficiente stock de \"{item.Name}\". Disponible: {item.Quantity}, solicitado: {r.Quantity}.");
            }

            item.Quantity -= r.Quantity;

            result.Add(new InvoiceItem
            {
                InventoryItemId = item.Id,
                Name = item.Name,
                Quantity = r.Quantity,
                UnitPrice = item.SalePrice,
                DiscountPercent = r.DiscountPercent,
                IvaRate = item.IvaRate,
                Exonerado = r.Exonerado,
            });
        }

        return (result, null);
    }
}