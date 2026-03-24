using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RideFlow.Core.Application.DTOs.Auth;
using RideFlow.Core.Application.Interfaces;
using RideFlow.Core.Persistence.Context;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RideFlow.Core.Application.Services;

public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;
    private readonly RideFlowDbContext _context;

    public AuthService(IConfiguration configuration, RideFlowDbContext context)
    {
        _configuration = configuration;
        _context = context;
    }

    public async Task<LoginResponseDto?> Login(LoginRequestDto request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Username == request.Username && x.IsActive);

        if (user == null)
            return null;

        var isValidPassword = false;

        if (!string.IsNullOrWhiteSpace(user.PasswordHash))
        {
            if (user.PasswordHash.StartsWith("$2"))
            {
                isValidPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            }
            else if (user.PasswordHash == request.Password)
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
                await _context.SaveChangesAsync();
                isValidPassword = true;
            }
        }

        if (!isValidPassword)
            return null;

        var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key not configured.");
        var expireHours = int.TryParse(_configuration["Jwt:ExpireHours"], out var hours) ? hours : 4;

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("FullName", user.FullName)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(expireHours),
            signingCredentials: creds
        );

        return new LoginResponseDto
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            FullName = user.FullName,
            Role = user.Role
        };
    }
}