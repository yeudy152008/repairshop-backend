using Microsoft.EntityFrameworkCore;
using RepairshopBackend.Application.DTOs;
using RepairshopBackend.Application.Interfaces;
using RepairshopBackend.Domain.Entities;
using RepairshopBackend.Infrastructure.Data;

namespace RepairshopBackend.Infrastructure.Services;

public class RoleService : IRoleService
{
    private readonly AppDbContext _context;

    public RoleService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<RoleDto>> GetAllAsync()
    {
        var roles = await _context.Roles
            .Include(r => r.RolePermissions)
            .OrderBy(r => r.Name)
            .ToListAsync();

        var allPermissions = await _context.Permissions.ToListAsync();

        return roles.Select(role => BuildDto(role, allPermissions)).ToList();
    }

    public async Task<(RoleDto? role, string? error)> CreateAsync(SaveRoleDto dto)
    {
        var duplicate = await _context.Roles.AnyAsync(r => r.Name == dto.Name);
        if (duplicate)
        {
            return (null, "Ya existe un rol con ese nombre.");
        }

        var role = new Role
        {
            Name = dto.Name,
            Description = dto.Description,
        };

        await AssignPermissionsAsync(role, dto.Permissions);

        _context.Roles.Add(role);
        await _context.SaveChangesAsync();

        var allPermissions = await _context.Permissions.ToListAsync();
        return (BuildDto(role, allPermissions), null);
    }

    public async Task<(RoleDto? role, string? error)> UpdateAsync(int id, SaveRoleDto dto)
    {
        var role = await _context.Roles
            .Include(r => r.RolePermissions)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (role is null)
        {
            return (null, "Rol no encontrado.");
        }

        var duplicate = await _context.Roles.AnyAsync(r => r.Name == dto.Name && r.Id != id);
        if (duplicate)
        {
            return (null, "Ya existe un rol con ese nombre.");
        }

        if (role.Name != dto.Name)
        {
            var usersWithRole = await _context.Users.Where(u => u.Role == role.Name).ToListAsync();
            foreach (var user in usersWithRole)
            {
                user.Role = dto.Name;
            }
        }

        role.Name = dto.Name;
        role.Description = dto.Description;

        _context.RolePermissions.RemoveRange(role.RolePermissions);
        role.RolePermissions.Clear();
        await AssignPermissionsAsync(role, dto.Permissions);

        await _context.SaveChangesAsync();

        var allPermissions = await _context.Permissions.ToListAsync();
        return (BuildDto(role, allPermissions), null);
    }

    public async Task<(bool success, string? error)> DeleteAsync(int id)
    {
        var role = await _context.Roles.FindAsync(id);
        if (role is null)
        {
            return (false, "Rol no encontrado.");
        }

        var inUse = await _context.Users.AnyAsync(u => u.Role == role.Name);
        if (inUse)
        {
            return (false, "No se puede eliminar: hay usuarios asignados a este rol.");
        }

        _context.Roles.Remove(role);
        await _context.SaveChangesAsync();
        return (true, null);
    }

    private async Task AssignPermissionsAsync(Role role, Dictionary<string, bool> permissions)
    {
        var grantedKeys = permissions.Where(p => p.Value).Select(p => p.Key).ToList();
        if (grantedKeys.Count == 0)
        {
            return;
        }

        var matchingPermissions = await _context.Permissions
            .Where(p => grantedKeys.Contains(p.Key))
            .ToListAsync();

        foreach (var permission in matchingPermissions)
        {
            role.RolePermissions.Add(new RolePermission { PermissionId = permission.Id });
        }
    }

    private static RoleDto BuildDto(Role role, List<Permission> allPermissions)
    {
        var grantedIds = role.RolePermissions.Select(rp => rp.PermissionId).ToHashSet();

        var permissions = allPermissions.ToDictionary(p => p.Key, p => grantedIds.Contains(p.Id));

        return new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            Permissions = permissions,
        };
    }
}