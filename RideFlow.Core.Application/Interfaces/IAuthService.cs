using RideFlow.Core.Application.DTOs.Auth;

namespace RideFlow.Core.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto?> Login(LoginRequestDto request);
}