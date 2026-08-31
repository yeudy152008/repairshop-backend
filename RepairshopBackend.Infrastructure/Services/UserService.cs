using Microsoft.EntityFrameworkCore;
using RepairshopBackend.Application.DTOs;
using RepairshopBackend.Application.Interfaces;
using RepairshopBackend.Domain.Entities;
using RepairshopBackend.Infrastructure.Data;

namespace RepairshopBackend.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<UserDto>> GetAllAsync()
    {
        return await _context.Users
            .OrderBy(u => u.FullName)
            .Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                FullName = u.FullName,
                Role = u.Role,
                Active = u.Active,
            })
            .ToListAsync();
    }

    public async Task<(UserDto? user, string? error)> CreateAsync(CreateUserDto dto)
    {
        var exists = await _context.Users.AnyAsync(u => u.Username == dto.Username);
        if (exists)
        {
            return (null, "Ya existe un usuario con ese nombre de usuario.");
        }

        var user = new User
        {
            Username = dto.Username,
            FullName = dto.FullName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = dto.Role,
            Active = dto.Active,
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return (new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            FullName = user.FullName,
            Role = user.Role,
            Active = user.Active,
        }, null);
    }

    public async Task<(UserDto? user, string? error)> UpdateAsync(UpdateUserDto dto)
    {
        var user = await _context.Users.FindAsync(dto.Id);
        if (user is null)
        {
            return (null, "Usuario no encontrado.");
        }

        user.FullName = dto.FullName;
        user.Role = dto.Role;
        user.Active = dto.Active;

        await _context.SaveChangesAsync();

        return (new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            FullName = user.FullName,
            Role = user.Role,
            Active = user.Active,
        }, null);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user is null)
        {
            return false;
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }
}