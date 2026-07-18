using Fym.Api.DTOs;
using Fym.Api.Data;      // <--- Asegúrate que tu AppDbContext esté aquí
using Fym.Api.Models;    // <--- Asegúrate que User, UserRole, Role estén aquí
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Fym.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;

    public UsuariosController(ApplicationDbContext context, IPasswordHasher<User> passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    [Authorize(Roles = "SuperAdmin")]
    [HttpPost("registrar")]
    public async Task<IActionResult> RegistrarUsuario([FromBody] RegistroDto dto)
    {
        // 1. Crear el usuario
       var user = new User { 
            Username = dto.Username, 
            Email = dto.Email,
            // Esto genera un hash compatible con tu método de Login actual
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password) 
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // 2. Asignar rol por defecto "User"
        var roleUser = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "User");
        if (roleUser != null)
        {
            _context.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = roleUser.Id });
            await _context.SaveChangesAsync();
        }

        return Ok(new { message = "Usuario registrado exitosamente" });
    }

    [Authorize(Roles = "SuperAdmin")]
    [HttpPost("{id}/asignar-rol")]
    public async Task<IActionResult> AsignarRol(Guid id, [FromBody] string nombreRol)
    {
        var user = await _context.Users.FindAsync(id);
        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == nombreRol);

        if (user == null || role == null) return NotFound("Usuario o Rol no encontrado");

        _context.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = role.Id });
        await _context.SaveChangesAsync();

        return Ok(new { message = $"Rol {nombreRol} asignado correctamente" });
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> ObtenerUsuarios()
    {
        var usuarios = await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .Select(u => new {
                u.Id,
                u.Username,
                Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList()
            }).ToListAsync();

        return Ok(usuarios);
    }
}