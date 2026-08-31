using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RepairshopBackend.Application.DTOs;
using RepairshopBackend.Application.Interfaces;
using RepairshopBackend.Application.Security;
using RepairshopBackend.Infrastructure.Data;

namespace RepairshopBackend.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IAuditLogService _auditLogService;

    public AuthService(AppDbContext context, IConfiguration configuration, IAuditLogService auditLogService)
    {
        _context = context;
        _configuration = configuration;
        _auditLogService = auditLogService;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username && u.Active);

        if (user is null)
        {
            return null;
        }

        bool passwordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if (!passwordValid)
        {
            return null;
        }

        var grantedPermissions = await GetGrantedPermissionKeysAsync(user.Role);
        var token = GenerateJwtToken(user.Id, user.Username, user.Role, grantedPermissions);

        await _auditLogService.RecordLoginAsync(user.Username);

        return new LoginResponseDto
        {
            Token = token,
            UserId = user.Id,
            Username = user.Username,
            FullName = user.FullName,
            Role = user.Role,
        };
    }

    private async Task<List<string>> GetGrantedPermissionKeysAsync(string roleName)
    {
        var role = await _context.Roles
            .Include(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(r => r.Name == roleName);

        return role?.RolePermissions.Select(rp => rp.Permission.Key).ToList() ?? new List<string>();
    }

    private string GenerateJwtToken(int userId, string username, string role, List<string> permissions)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role),
        };

        claims.AddRange(permissions.Select(p => new Claim(PermissionKeys.ClaimType, p)));

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}