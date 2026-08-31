using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepairshopBackend.Application.DTOs;
using RepairshopBackend.Application.Interfaces;

namespace RepairshopBackend.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SuppliersController : ControllerBase
{
    private readonly ISupplierService _supplierService;

    public SuppliersController(ISupplierService supplierService)
    {
        _supplierService = supplierService;
    }

    [HttpGet]
    [Authorize(Policy = "suppliers.read")]
    public async Task<IActionResult> GetAll()
    {
        var suppliers = await _supplierService.GetAllAsync();
        return Ok(suppliers);
    }

    [HttpPost]
    [Authorize(Policy = "suppliers.create")]
    public async Task<IActionResult> Create([FromBody] SaveSupplierDto dto)
    {
        var (supplier, error) = await _supplierService.CreateAsync(dto);
        if (error is not null)
        {
            return BadRequest(new { message = error });
        }
        return Ok(supplier);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "suppliers.update")]
    public async Task<IActionResult> Update(int id, [FromBody] SaveSupplierDto dto)
    {
        var (supplier, error) = await _supplierService.UpdateAsync(id, dto);
        if (error is not null)
        {
            return BadRequest(new { message = error });
        }
        return Ok(supplier);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "suppliers.delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var (success, error) = await _supplierService.DeleteAsync(id);
        if (!success)
        {
            return BadRequest(new { message = error });
        }
        return NoContent();
    }
}