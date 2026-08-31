using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepairshopBackend.Application.DTOs;
using RepairshopBackend.Application.Interfaces;

namespace RepairshopBackend.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VehiclesController : ControllerBase
{
    private readonly IVehicleService _vehicleService;

    public VehiclesController(IVehicleService vehicleService)
    {
        _vehicleService = vehicleService;
    }

    [HttpGet]
    [Authorize(Policy = "vehicles.read")]
    public async Task<IActionResult> GetAll()
    {
        var vehicles = await _vehicleService.GetAllAsync();
        return Ok(vehicles);
    }

    [HttpPost]
    [Authorize(Policy = "vehicles.create")]
    public async Task<IActionResult> Create([FromBody] SaveVehicleDto dto)
    {
        var (vehicle, error) = await _vehicleService.CreateAsync(dto);
        if (error is not null)
        {
            return BadRequest(new { message = error });
        }
        return Ok(vehicle);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "vehicles.update")]
    public async Task<IActionResult> Update(int id, [FromBody] SaveVehicleDto dto)
    {
        var (vehicle, error) = await _vehicleService.UpdateAsync(id, dto);
        if (error is not null)
        {
            return BadRequest(new { message = error });
        }
        return Ok(vehicle);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "vehicles.delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var (success, error) = await _vehicleService.DeleteAsync(id);
        if (!success)
        {
            return BadRequest(new { message = error });
        }
        return NoContent();
    }
}