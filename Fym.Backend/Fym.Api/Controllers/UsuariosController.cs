using Fym.Api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fym.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // 🔒 Exige Token JWT válido para CUALQUIER método de este controlador
public class UsuariosController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public UsuariosController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/usuarios
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        // Traemos los usuarios de la BD omitiendo el PasswordHash por seguridad
        var usuarios = await _context.Users
            .Select(u => new {
                u.Id,
                u.Username,
                u.Email
            })
            .ToListAsync();

        return Ok(usuarios);
    }

    // GET: api/usuarios/perfil
    [HttpGet("perfil")]
    public IActionResult GetPerfil()
    {
        // Ejemplo de cómo leer los claims que vienen DENTRO del token que envió el cliente
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var username = User.Identity?.Name;

        return Ok(new {
            message = "Acceso autorizado al perfil",
            userId = userId,
            username = username
        });
    }
}