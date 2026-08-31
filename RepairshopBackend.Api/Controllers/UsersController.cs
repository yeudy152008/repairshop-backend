using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepairshopBackend.Application.DTOs;
using RepairshopBackend.Application.Interfaces;

namespace RepairshopBackend.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [Authorize(Policy = "users.read")]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userService.GetAllAsync();
        return Ok(users);
    }

    [HttpPost]
    [Authorize(Policy = "users.create")]
    public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
    {
        var (user, error) = await _userService.CreateAsync(dto);
        if (error is not null)
        {
            return BadRequest(new { message = error });
        }
        return Ok(user);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "users.update")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto dto)
    {
        if (id != dto.Id)
        {
            return BadRequest(new { message = "El ID de la ruta no coincide con el del cuerpo." });
        }

        var (user, error) = await _userService.UpdateAsync(dto);
        if (error is not null)
        {
            return NotFound(new { message = error });
        }
        return Ok(user);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "users.delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _userService.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound(new { message = "Usuario no encontrado." });
        }
        return NoContent();
    }
}