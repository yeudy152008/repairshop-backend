using Microsoft.EntityFrameworkCore;
using RepairshopBackend.Application.DTOs;
using RepairshopBackend.Application.Interfaces;
using RepairshopBackend.Domain.Entities;
using RepairshopBackend.Infrastructure.Data;

namespace RepairshopBackend.Infrastructure.Services;

public class WorkOrderService : IWorkOrderService
{
    private readonly AppDbContext _context;

    public WorkOrderService(AppDbContext context)
    {
        _context = context;
    }

    private static WorkOrderDto ToDto(WorkOrder o) => new()
    {
        Id = o.Id,
        VehicleId = o.VehicleId,
        VehiclePlate = o.VehiclePlate,
        VehicleModel = o.VehicleModel,
        CustomerName = o.Vehicle?.Customer?.FullName ?? string.Empty,
        Technician = o.Technician,
        Description = o.Description,
        LaborCost = o.LaborCost,
        HoursSpent = o.HoursSpent,
        Status = o.Status,
        CreatedAt = o.CreatedAt,
        Parts = o.Parts.Select(p => new WorkOrderPartDto
        {
            Id = p.Id,
            InventoryItemId = p.InventoryItemId,
            Name = p.Name,
            Quantity = p.Quantity,
            Cost = p.Cost,
        }).ToList(),
    };

    public async Task<List<WorkOrderDto>> GetAllAsync()
    {
        var orders = await _context.WorkOrders
            .Include(o => o.Parts)
            .Include(o => o.Vehicle)
            .ThenInclude(v => v.Customer)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        return orders.Select(ToDto).ToList();
    }

    public async Task<(WorkOrderDto? order, string? error)> CreateAsync(CreateWorkOrderDto dto)
    {
        var vehicle = await _context.Vehicles
            .Include(v => v.Customer)
            .FirstOrDefaultAsync(v => v.Id == dto.VehicleId);

        if (vehicle is null)
        {
            return (null, "El vehículo seleccionado no existe.");
        }

        var (parts, partsError) = await BuildPartsAndReserveStockAsync(dto.Parts);
        if (partsError is not null)
        {
            return (null, partsError);
        }

        var order = new WorkOrder
        {
            VehicleId = vehicle.Id,
            VehiclePlate = vehicle.Plate,
            VehicleModel = $"{vehicle.Brand} {vehicle.Model} {vehicle.Year}",
            Technician = dto.Technician,
            Description = dto.Description,
            LaborCost = dto.LaborCost,
            HoursSpent = dto.HoursSpent,
            Status = dto.Status,
            CreatedAt = DateTime.UtcNow,
            Parts = parts,
        };

        _context.WorkOrders.Add(order);
        await _context.SaveChangesAsync();

        order.Vehicle = vehicle;
        return (ToDto(order), null);
    }

    public async Task<(WorkOrderDto? order, string? error)> UpdateAsync(UpdateWorkOrderDto dto)
    {
        var order = await _context.WorkOrders
            .Include(o => o.Parts)
            .Include(o => o.Vehicle)
            .ThenInclude(v => v.Customer)
            .FirstOrDefaultAsync(o => o.Id == dto.Id);

        if (order is null)
        {
            return (null, "Orden de trabajo no encontrada.");
        }

        if (order.Status == "Finalizada")
        {
            return (null, "Esta orden ya está finalizada y no se puede modificar.");
        }

        await RestoreStockAsync(order.Parts);

        var (newParts, partsError) = await BuildPartsAndReserveStockAsync(dto.Parts);
        if (partsError is not null)
        {
            return (null, partsError);
        }

        order.Technician = dto.Technician;
        order.Description = dto.Description;
        order.LaborCost = dto.LaborCost;
        order.HoursSpent = dto.HoursSpent;
        order.Status = dto.Status;

        _context.WorkOrderParts.RemoveRange(order.Parts);
        order.Parts.Clear();
        foreach (var part in newParts)
        {
            order.Parts.Add(part);
        }

        await _context.SaveChangesAsync();

        return (ToDto(order), null);
    }

    public async Task<(bool success, string? error)> DeleteAsync(int id)
    {
        var order = await _context.WorkOrders
            .Include(o => o.Parts)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order is null)
        {
            return (false, "Orden de trabajo no encontrada.");
        }

        if (order.Status == "Finalizada")
        {
            return (false, "No se puede eliminar una orden finalizada.");
        }

        await RestoreStockAsync(order.Parts);

        _context.WorkOrders.Remove(order);
        await _context.SaveChangesAsync();
        return (true, null);
    }

    private async Task<(List<WorkOrderPart> parts, string? error)> BuildPartsAndReserveStockAsync(List<CreatePartDto> requestedParts)
    {
        var result = new List<WorkOrderPart>();

        foreach (var requested in requestedParts)
        {
            if (requested.Quantity <= 0)
            {
                return (result, "La cantidad de cada repuesto debe ser mayor a cero.");
            }

            var item = await _context.InventoryItems.FindAsync(requested.InventoryItemId);
            if (item is null)
            {
                return (result, "Uno de los repuestos seleccionados ya no existe en el inventario.");
            }

            if (item.Quantity < requested.Quantity)
            {
                return (result, $"No hay suficiente stock de \"{item.Name}\". Disponible: {item.Quantity}, solicitado: {requested.Quantity}.");
            }

            item.Quantity -= requested.Quantity;

            result.Add(new WorkOrderPart
            {
                InventoryItemId = item.Id,
                Name = item.Name,
                Quantity = requested.Quantity,
                Cost = item.UnitCost,
            });
        }

        return (result, null);
    }

    private async Task RestoreStockAsync(IEnumerable<WorkOrderPart> parts)
    {
        foreach (var part in parts)
        {
            var item = await _context.InventoryItems.FindAsync(part.InventoryItemId);
            if (item is not null)
            {
                item.Quantity += part.Quantity;
            }
        }
    }
}