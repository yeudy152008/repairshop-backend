using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepairshopBackend.Application.DTOs;
using RepairshopBackend.Application.Interfaces;

namespace RepairshopBackend.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RolesController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpGet]
    [Authorize(Policy = "roles.read")]
    public async Task<IActionResult> GetAll()
    {
        var roles = await _roleService.GetAllAsync();
        return Ok(roles);
    }

    [HttpPost]
    [Authorize(Policy = "roles.create")]
    public async Task<IActionResult> Create([FromBody] SaveRoleDto dto)
    {
        var (role, error) = await _roleService.CreateAsync(dto);
        if (error is not null)
        {
            return BadRequest(new { message = error });
        }
        return Ok(role);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "roles.update")]
    public async Task<IActionResult> Update(int id, [FromBody] SaveRoleDto dto)
    {
        var (role, error) = await _roleService.UpdateAsync(id, dto);
        if (error is not null)
        {
            return BadRequest(new { message = error });
        }
        return Ok(role);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "roles.delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var (success, error) = await _roleService.DeleteAsync(id);
        if (!success)
        {
            return BadRequest(new { message = error });
        }
        return NoContent();
    }
}