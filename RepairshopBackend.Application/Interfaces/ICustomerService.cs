using RepairshopBackend.Application.DTOs;

namespace RepairshopBackend.Application.Interfaces;

public interface ICustomerService
{
    Task<List<CustomerDto>> GetAllAsync();
    Task<(CustomerDto? customer, string? error)> CreateAsync(CreateCustomerDto dto);
    Task<(CustomerDto? customer, string? error)> UpdateAsync(UpdateCustomerDto dto);
    Task<(bool success, string? error)> DeleteAsync(int id);
}