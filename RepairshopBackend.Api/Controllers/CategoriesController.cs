using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepairshopBackend.Application.DTOs;
using RepairshopBackend.Application.Interfaces;

namespace RepairshopBackend.Api.Controllers;

[ApiController]
[Route("api/inventory-categories")]
[Authorize]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    [Authorize(Policy = "inventory.read")]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _categoryService.GetAllAsync();
        return Ok(categories);
    }

    [HttpPost]
    [Authorize(Policy = "inventory.create")]
    public async Task<IActionResult> Create([FromBody] SaveCategoryDto dto)
    {
        var (category, error) = await _categoryService.CreateAsync(dto);
        if (error is not null)
        {
            return BadRequest(new { message = error });
        }
        return Ok(category);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "inventory.update")]
    public async Task<IActionResult> Update(int id, [FromBody] SaveCategoryDto dto)
    {
        var (category, error) = await _categoryService.UpdateAsync(id, dto);
        if (error is not null)
        {
            return BadRequest(new { message = error });
        }
        return Ok(category);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "inventory.delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var (success, error) = await _categoryService.DeleteAsync(id);
        if (!success)
        {
            return BadRequest(new { message = error });
        }
        return NoContent();
    }
}