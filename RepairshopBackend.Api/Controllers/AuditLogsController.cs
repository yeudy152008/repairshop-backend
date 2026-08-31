using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepairshopBackend.Application.DTOs;
using RepairshopBackend.Application.Interfaces;

namespace RepairshopBackend.Api.Controllers;

[ApiController]
[Route("api")]
[Authorize]
public class AuditLogsController : ControllerBase
{
    private readonly IAuditLogService _auditLogService;

    public AuditLogsController(IAuditLogService auditLogService)
    {
        _auditLogService = auditLogService;
    }

    [HttpGet("access-logs")]
    [Authorize(Policy = "logs.read")]
    public async Task<IActionResult> GetAccessLogs()
    {
        var logs = await _auditLogService.GetAccessLogsAsync();
        return Ok(logs);
    }

    [HttpGet("movement-logs")]
    [Authorize(Policy = "logs.read")]
    public async Task<IActionResult> GetMovementLogs()
    {
        var logs = await _auditLogService.GetMovementLogsAsync();
        return Ok(logs);
    }

    [HttpPost("movement-logs")]
    public async Task<IActionResult> RecordMovement([FromBody] CreateMovementLogDto dto)
    {
        var username = User.Identity?.Name ?? "sistema";
        await _auditLogService.RecordMovementAsync(username, dto);
        return NoContent();
    }
}