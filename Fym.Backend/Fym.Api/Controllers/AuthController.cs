using Fym.Api.DTOs;
using Fym.Api.Services;
using Fym.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Fym.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var response = await _authService.LoginAsync(request);

        if (response == null)
        {
            // Mantenemos el mensaje genérico por seguridad
            return Unauthorized(new { message = "Credenciales incorrectas o usuario no encontrado." });
        }

        return Ok(response);
    }

    
}