using Microsoft.EntityFrameworkCore;
using RepairshopBackend.Application.DTOs;
using RepairshopBackend.Application.Interfaces;
using RepairshopBackend.Domain.Entities;
using RepairshopBackend.Infrastructure.Data;

namespace RepairshopBackend.Infrastructure.Services;

public class VehicleService : IVehicleService
{
    private readonly AppDbContext _context;

    public VehicleService(AppDbContext context)
    {
        _context = context;
    }

    private static VehicleDto ToDto(Vehicle v) => new()
    {
        Id = v.Id,
        CustomerId = v.CustomerId,
        CustomerName = v.Customer?.FullName ?? string.Empty,
        Brand = v.Brand,
        Model = v.Model,
        Year = v.Year,
        Plate = v.Plate,
    };

    public async Task<List<VehicleDto>> GetAllAsync()
    {
        var vehicles = await _context.Vehicles
            .Include(v => v.Customer)
            .OrderBy(v => v.Plate)
            .ToListAsync();

        return vehicles.Select(ToDto).ToList();
    }

    public async Task<(VehicleDto? vehicle, string? error)> CreateAsync(SaveVehicleDto dto)
    {
        var customer = await _context.Customers.FindAsync(dto.CustomerId);
        if (customer is null)
        {
            return (null, "El cliente seleccionado no existe.");
        }

        var duplicatePlate = await _context.Vehicles.AnyAsync(v => v.Plate == dto.Plate);
        if (duplicatePlate)
        {
            return (null, "Ya existe un vehículo registrado con esta placa.");
        }

        var vehicle = new Vehicle
        {
            CustomerId = dto.CustomerId,
            Brand = dto.Brand,
            Model = dto.Model,
            Year = dto.Year,
            Plate = dto.Plate,
        };

        _context.Vehicles.Add(vehicle);
        await _context.SaveChangesAsync();

        vehicle.Customer = customer;
        return (ToDto(vehicle), null);
    }

    public async Task<(VehicleDto? vehicle, string? error)> UpdateAsync(int id, SaveVehicleDto dto)
    {
        var vehicle = await _context.Vehicles
            .Include(v => v.Customer)
            .FirstOrDefaultAsync(v => v.Id == id);

        if (vehicle is null)
        {
            return (null, "Vehículo no encontrado.");
        }

        var duplicatePlate = await _context.Vehicles.AnyAsync(v => v.Plate == dto.Plate && v.Id != id);
        if (duplicatePlate)
        {
            return (null, "Ya existe un vehículo registrado con esta placa.");
        }

        vehicle.Brand = dto.Brand;
        vehicle.Model = dto.Model;
        vehicle.Year = dto.Year;
        vehicle.Plate = dto.Plate;

        await _context.SaveChangesAsync();

        return (ToDto(vehicle), null);
    }

    public async Task<(bool success, string? error)> DeleteAsync(int id)
    {
        var vehicle = await _context.Vehicles.FindAsync(id);
        if (vehicle is null)
        {
            return (false, "Vehículo no encontrado.");
        }

        var hasOrders = await _context.WorkOrders.AnyAsync(o => o.VehicleId == id);
        if (hasOrders)
        {
            return (false, "No se puede eliminar: este vehículo tiene órdenes de trabajo asociadas.");
        }

        _context.Vehicles.Remove(vehicle);
        await _context.SaveChangesAsync();
        return (true, null);
    }
}