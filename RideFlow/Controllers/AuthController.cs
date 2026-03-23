using Microsoft.AspNetCore.Mvc;
using RideFlow.Core.Application.DTOs.Auth;
using RideFlow.Core.Application.Interfaces;

namespace RideFlow.Core.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _service;

    public AuthController(IAuthService service)
    {
        _service = service;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequestDto request)
    {
        var result = await _service.Login(request);

        if (result == null)
            return Unauthorized();

        return Ok(result);
    }
}