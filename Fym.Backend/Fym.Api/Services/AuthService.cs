using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Fym.Api.Data;
using Fym.Api.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Fym.Api.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        // 1. Buscar al usuario por Email o Username, e incluir sus roles a través de la tabla asociativa
        var user = await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Email == request.UsernameOrEmail || u.Username == request.UsernameOrEmail);

        if (user == null)
        {
            return null; // El usuario no existe
        }

        // 2. Verificar la contraseña usando BCrypt
        // Nota: Asegúrate de tener instalado el paquete BCrypt.Net-Next (si no, lo agregamos ahora)
        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        
        if (!isPasswordValid)
        {
            return null; // Contraseña incorrecta
        }

        // 3. Extraer la lista de nombres de los roles del usuario
        var userRoles = user.UserRoles.Select(ur => ur.Role.Name).ToList();

        // 4. Generar el Token JWT
        var token = GenerateJwtToken(user, userRoles);

        // 5. Retornar la respuesta estructurada
        return new AuthResponse
        {
            Token = token,
            Username = user.Username,
            Email = user.Email,
            Roles = userRoles
        };
    }

    private string GenerateJwtToken(Models.User user, List<string> roles)
        {
            // 1. Leer los valores directamente usando la ruta con dos puntos (:)
            var secret = _configuration["JwtSettings:Secret"] 
                ?? throw new InvalidOperationException("La clave secreta de JWT no está configurada.");
            
            var issuer = _configuration["JwtSettings:Issuer"];
            var audience = _configuration["JwtSettings:Audience"];
            
            // Si no se configura la expiración, por defecto le damos 60 minutos
            var expiryInMinutesString = _configuration["JwtSettings:ExpiryInMinutes"];
            int expiryInMinutes = int.TryParse(expiryInMinutesString, out var parsedMinutes) ? parsedMinutes : 60;

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 2. Definir los Claims (Carga útil del token)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email)
            };

            // Agregar cada rol como un claim independiente de tipo Role
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var expiry = DateTime.UtcNow.AddMinutes(expiryInMinutes);

            // 3. Crear el Descriptor del Token con los strings limpios
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiry,
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
}