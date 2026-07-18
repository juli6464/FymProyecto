using System;
using System.Collections.Generic;

namespace Fym.Api.Models;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Propiedad de navegación para la relación M:N
    public List<UserRole> UserRoles { get; set; } = new();
}