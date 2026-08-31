using RepairshopBackend.Application.DTOs;

namespace RepairshopBackend.Application.Interfaces;

public interface IAuditLogService
{
    Task RecordLoginAsync(string username);
    Task RecordLogoutAsync(string username);
    Task RecordMovementAsync(string username, CreateMovementLogDto dto);
    Task<List<AccessLogDto>> GetAccessLogsAsync();
    Task<List<MovementLogDto>> GetMovementLogsAsync();
}