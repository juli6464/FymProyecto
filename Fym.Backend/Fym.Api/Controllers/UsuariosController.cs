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
        try
        {
            var user = new User { 
                Username = dto.Username, 
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password) 
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var roleUser = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "User");
            if (roleUser != null)
            {
                _context.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = roleUser.Id });
                await _context.SaveChangesAsync();
            }

            return Ok(new { message = "Usuario registrado exitosamente" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al registrar usuario", details = ex.Message });
        }
    }
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> ObtenerUsuarios()
    {
        try
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
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al obtener la lista de usuarios", details = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var usuario = await _context.Users
                .Where(u => u.Id == id)
                .Select(u => new {
                    u.Username,
                    u.Email,
                    Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList()
                })
                .FirstOrDefaultAsync();

            if (usuario == null) return NotFound("Usuario no encontrado");

            return Ok(usuario);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al obtener el usuario", details = ex.Message });
        }
    }
    [Authorize(Roles = "SuperAdmin")]
    [HttpPost("{id}/asignar-rol")]
    public async Task<IActionResult> AsignarRol(Guid id, [FromBody] string nombreRol)
    {
        // Usamos una transacción para asegurar que o se hace todo o no se hace nada
        using var transaction = await _context.Database.BeginTransactionAsync();
        
        try {
            var user = await _context.Users.FindAsync(id);
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == nombreRol);

            if (user == null || role == null) return NotFound("Usuario o Rol no encontrado");

            // 1. Buscamos TODOS los registros de roles de este usuario
            var rolesActuales = await _context.UserRoles
                .Where(ur => ur.UserId == id)
                .ToListAsync();

            // 2. Eliminamos todos los roles que tenga actualmente
            if (rolesActuales.Any())
            {
                _context.UserRoles.RemoveRange(rolesActuales);
            }

            // 3. Agregamos el nuevo rol
            _context.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = role.Id });

            // 4. Guardamos cambios
            await _context.SaveChangesAsync();
            
            // 5. Confirmamos la transacción
            await transaction.CommitAsync();

            return Ok(new { message = $"Rol {nombreRol} asignado exitosamente y roles anteriores eliminados." });
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            return StatusCode(500, "Error al actualizar el rol.");
        }
    }
}