using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepairshopBackend.Application.DTOs;
using RepairshopBackend.Application.Interfaces;

namespace RepairshopBackend.Api.Controllers;

[ApiController]
[Route("api/inventory")]
[Authorize]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;

    public InventoryController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    [HttpGet]
    [Authorize(Policy = "inventory.read")]
    public async Task<IActionResult> GetAll()
    {
        var items = await _inventoryService.GetAllAsync();
        return Ok(items);
    }

    [HttpPost]
    [Authorize(Policy = "inventory.create")]
    public async Task<IActionResult> Create([FromBody] SaveInventoryItemDto dto)
    {
        var (item, error) = await _inventoryService.CreateAsync(dto);
        if (error is not null)
        {
            return BadRequest(new { message = error });
        }
        return Ok(item);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "inventory.update")]
    public async Task<IActionResult> Update(int id, [FromBody] SaveInventoryItemDto dto)
    {
        var (item, error) = await _inventoryService.UpdateAsync(id, dto);
        if (error is not null)
        {
            return BadRequest(new { message = error });
        }
        return Ok(item);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "inventory.delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var (success, error) = await _inventoryService.DeleteAsync(id);
        if (!success)
        {
            return BadRequest(new { message = error });
        }
        return NoContent();
    }
}