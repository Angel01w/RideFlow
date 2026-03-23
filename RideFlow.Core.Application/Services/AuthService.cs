using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RideFlow.Core.Application.DTOs.Auth;
using RideFlow.Core.Application.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RideFlow.Core.Application.Services;

public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;

    public AuthService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task<LoginResponseDto?> Login(LoginRequestDto request)
    {
        if (request.Username != "angel.morel" || request.Password != "123456")
            return Task.FromResult<LoginResponseDto?>(null);

        var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key not configured.");
        var expireHours = int.TryParse(_configuration["Jwt:ExpireHours"], out var hours) ? hours : 4;

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, request.Username),
            new Claim(ClaimTypes.Role, "Admin"),
            new Claim("FullName", "Angel Morel")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(expireHours),
            signingCredentials: creds
        );

        return Task.FromResult<LoginResponseDto?>(new LoginResponseDto
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            FullName = "Angel Morel",
            Role = "Admin"
        });
    }
}