using System;
using System.Collections.Generic;

namespace Fym.Api.Models;

public class Role
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty; // Ej: "SuperAdmin", "User"
    public string Description { get; set; } = string.Empty;

    // Propiedad de navegación para la relación M:N
    public List<UserRole> UserRoles { get; set; } = new();
}