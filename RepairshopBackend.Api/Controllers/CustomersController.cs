using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepairshopBackend.Application.DTOs;
using RepairshopBackend.Application.Interfaces;

namespace RepairshopBackend.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet]
    [Authorize(Policy = "customers.read")]
    public async Task<IActionResult> GetAll()
    {
        var customers = await _customerService.GetAllAsync();
        return Ok(customers);
    }

    [HttpPost]
    [Authorize(Policy = "customers.create")]
    public async Task<IActionResult> Create([FromBody] CreateCustomerDto dto)
    {
        var (customer, error) = await _customerService.CreateAsync(dto);
        if (error is not null)
        {
            return BadRequest(new { message = error });
        }
        return Ok(customer);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "customers.update")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCustomerDto dto)
    {
        if (id != dto.Id)
        {
            return BadRequest(new { message = "El ID de la ruta no coincide con el del cuerpo." });
        }

        var (customer, error) = await _customerService.UpdateAsync(dto);
        if (error is not null)
        {
            return BadRequest(new { message = error });
        }
        return Ok(customer);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "customers.delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var (success, error) = await _customerService.DeleteAsync(id);
        if (!success)
        {
            return BadRequest(new { message = error });
        }
        return NoContent();
    }
}