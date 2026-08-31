using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepairshopBackend.Application.DTOs;
using RepairshopBackend.Application.Interfaces;

namespace RepairshopBackend.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IAuditLogService _auditLogService;

    public AuthController(IAuthService authService, IAuditLogService auditLogService)
    {
        _authService = authService;
        _auditLogService = auditLogService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var result = await _authService.LoginAsync(request);

        if (result is null)
        {
            return Unauthorized(new { message = "Usuario o contraseña incorrectos." });
        }

        return Ok(result);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var username = User.Identity?.Name;
        if (!string.IsNullOrEmpty(username))
        {
            await _auditLogService.RecordLogoutAsync(username);
        }
        return NoContent();
    }
}