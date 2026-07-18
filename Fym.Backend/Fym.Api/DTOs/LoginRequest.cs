using System.ComponentModel.DataAnnotations;

namespace Fym.Api.DTOs;

public class LoginRequest
{
    [Required(ErrorMessage = "El correo electrónico o usuario es requerido.")]
    public string UsernameOrEmail { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es requerida.")]
    public string Password { get; set; } = string.Empty;
}