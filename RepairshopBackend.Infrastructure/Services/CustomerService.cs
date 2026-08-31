using Microsoft.EntityFrameworkCore;
using RepairshopBackend.Application.DTOs;
using RepairshopBackend.Application.Interfaces;
using RepairshopBackend.Domain.Entities;
using RepairshopBackend.Infrastructure.Data;

namespace RepairshopBackend.Infrastructure.Services;

public class CustomerService : ICustomerService
{
    private readonly AppDbContext _context;

    public CustomerService(AppDbContext context)
    {
        _context = context;
    }

    private static CustomerDto ToDto(Customer c) => new()
    {
        Id = c.Id,
        FullName = c.FullName,
        IdNumber = c.IdNumber,
        Phone = c.Phone,
        Email = c.Email,
        ExemptionLetterNumber = c.ExemptionLetterNumber,
        Active = c.Active,
    };

    public async Task<List<CustomerDto>> GetAllAsync()
    {
        return await _context.Customers
            .OrderBy(c => c.FullName)
            .Select(c => ToDto(c))
            .ToListAsync();
    }

    public async Task<(CustomerDto? customer, string? error)> CreateAsync(CreateCustomerDto dto)
    {
        var duplicateId = await _context.Customers.AnyAsync(c => c.IdNumber == dto.IdNumber);
        if (duplicateId)
        {
            return (null, "Ya existe un cliente registrado con esta cédula.");
        }

        var customer = new Customer
        {
            FullName = dto.FullName,
            IdNumber = dto.IdNumber,
            Phone = dto.Phone,
            Email = dto.Email,
            ExemptionLetterNumber = dto.ExemptionLetterNumber,
            Active = dto.Active,
        };

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();

        return (ToDto(customer), null);
    }

    public async Task<(CustomerDto? customer, string? error)> UpdateAsync(UpdateCustomerDto dto)
    {
        var customer = await _context.Customers.FindAsync(dto.Id);
        if (customer is null)
        {
            return (null, "Cliente no encontrado.");
        }

        var duplicateId = await _context.Customers.AnyAsync(c => c.IdNumber == dto.IdNumber && c.Id != dto.Id);
        if (duplicateId)
        {
            return (null, "Ya existe un cliente registrado con esta cédula.");
        }

        customer.FullName = dto.FullName;
        customer.IdNumber = dto.IdNumber;
        customer.Phone = dto.Phone;
        customer.Email = dto.Email;
        customer.ExemptionLetterNumber = dto.ExemptionLetterNumber;
        customer.Active = dto.Active;

        await _context.SaveChangesAsync();

        return (ToDto(customer), null);
    }

    public async Task<(bool success, string? error)> DeleteAsync(int id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer is null)
        {
            return (false, "Cliente no encontrado.");
        }

        var vehicleIds = await _context.Vehicles
            .Where(v => v.CustomerId == id)
            .Select(v => v.Id)
            .ToListAsync();

        var hasOrders = await _context.WorkOrders.AnyAsync(o => vehicleIds.Contains(o.VehicleId));
        if (hasOrders)
        {
            return (false, "No se puede eliminar: este cliente tiene vehículos con órdenes de trabajo asociadas.");
        }

        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();
        return (true, null);
    }
}