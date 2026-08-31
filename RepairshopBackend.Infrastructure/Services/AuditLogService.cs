using Microsoft.EntityFrameworkCore;
using RepairshopBackend.Application.DTOs;
using RepairshopBackend.Application.Interfaces;
using RepairshopBackend.Domain.Entities;
using RepairshopBackend.Infrastructure.Data;

namespace RepairshopBackend.Infrastructure.Services;

public class AuditLogService : IAuditLogService
{
    private readonly AppDbContext _context;

    public AuditLogService(AppDbContext context)
    {
        _context = context;
    }

    public async Task RecordLoginAsync(string username)
    {
        _context.AccessLogs.Add(new AccessLog
        {
            Username = username,
            LoginAt = DateTime.UtcNow,
            LogoutAt = null,
        });
        await _context.SaveChangesAsync();
    }

    public async Task RecordLogoutAsync(string username)
    {
        var openSession = await _context.AccessLogs
            .Where(a => a.Username == username && a.LogoutAt == null)
            .OrderByDescending(a => a.LoginAt)
            .FirstOrDefaultAsync();

        if (openSession is not null)
        {
            openSession.LogoutAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task RecordMovementAsync(string username, CreateMovementLogDto dto)
    {
        _context.MovementLogs.Add(new MovementLog
        {
            Username = username,
            Timestamp = DateTime.UtcNow,
            Type = dto.Type,
            Module = dto.Module,
            Detail = dto.Detail,
        });
        await _context.SaveChangesAsync();
    }

    public async Task<List<AccessLogDto>> GetAccessLogsAsync()
    {
        return await _context.AccessLogs
            .OrderByDescending(a => a.LoginAt)
            .Select(a => new AccessLogDto
            {
                Id = a.Id,
                Username = a.Username,
                LoginAt = a.LoginAt,
                LogoutAt = a.LogoutAt,
            })
            .ToListAsync();
    }

    public async Task<List<MovementLogDto>> GetMovementLogsAsync()
    {
        return await _context.MovementLogs
            .OrderByDescending(m => m.Timestamp)
            .Select(m => new MovementLogDto
            {
                Id = m.Id,
                Username = m.Username,
                Timestamp = m.Timestamp,
                Type = m.Type,
                Module = m.Module,
                Detail = m.Detail,
            })
            .ToListAsync();
    }
}