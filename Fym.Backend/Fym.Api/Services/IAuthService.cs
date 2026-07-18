using Fym.Api.DTOs;

namespace Fym.Api.Services;

public interface IAuthService
{
    Task<AuthResponse?> LoginAsync(LoginRequest request);
}