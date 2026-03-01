using Soat.Eleven.Kutcut.Users.Domain.Entities;

namespace Soat.Eleven.Kutcut.Users.Domain.Interfaces.Services;

public interface IJwtTokenService
{
    /// <summary>
    /// Gera JWT Token para usuário
    /// </summary>
    /// <param name="user"></param>
    /// <returns>JWT (string)</returns>
    string GenerateToken(User user);
    Guid GetUsuarioId();
}
