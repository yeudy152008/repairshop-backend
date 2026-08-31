using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepairshopBackend.Application.DTOs;
using RepairshopBackend.Application.Interfaces;

namespace RepairshopBackend.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WorkOrdersController : ControllerBase
{
    private readonly IWorkOrderService _workOrderService;

    public WorkOrdersController(IWorkOrderService workOrderService)
    {
        _workOrderService = workOrderService;
    }

    [HttpGet]
    [Authorize(Policy = "orders.read")]
    public async Task<IActionResult> GetAll()
    {
        var orders = await _workOrderService.GetAllAsync();
        return Ok(orders);
    }

    [HttpPost]
    [Authorize(Policy = "orders.create")]
    public async Task<IActionResult> Create([FromBody] CreateWorkOrderDto dto)
    {
        var (order, error) = await _workOrderService.CreateAsync(dto);
        if (error is not null)
        {
            return BadRequest(new { message = error });
        }
        return Ok(order);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "orders.update")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateWorkOrderDto dto)
    {
        if (id != dto.Id)
        {
            return BadRequest(new { message = "El ID de la ruta no coincide con el del cuerpo." });
        }

        var (order, error) = await _workOrderService.UpdateAsync(dto);
        if (error is not null)
        {
            return BadRequest(new { message = error });
        }
        return Ok(order);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "orders.delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var (success, error) = await _workOrderService.DeleteAsync(id);
        if (!success)
        {
            return BadRequest(new { message = error });
        }
        return NoContent();
    }
}