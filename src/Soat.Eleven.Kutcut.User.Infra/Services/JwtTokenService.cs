using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Soat.Eleven.Kutcut.Users.Domain.Entities;
using Soat.Eleven.Kutcut.Users.Domain.Interfaces.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Soat.Eleven.Kutcut.Users.Infra.Services;

public class JwtTokenService : IJwtTokenService
{
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly IConfiguration configuration;

    public JwtTokenService(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
    {
        this.httpContextAccessor = httpContextAccessor;
        this.configuration = configuration;
    }

    public Guid GetUsuarioId()
    {
        var id = ReadToken(JwtRegisteredClaimNames.Sub);

        return Guid.Parse(id);
    }

    private string ReadToken(string typeClaim)
    {
        var token = httpContextAccessor.HttpContext?.Request.Headers.Authorization.ElementAtOrDefault(0) ?? throw new AuthenticationFailureException("Usuário não autenticado");

        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(token.Replace("Bearer ", string.Empty)) ?? throw new AuthenticationFailureException("Usuário não autenticado");

        return (jsonToken as JwtSecurityToken)!.Claims.First(x => x.Type == typeClaim).Value;
    }
    public string GenerateToken(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(configuration["JwtSettings:SecretKey"]!);
        var expirationDate = DateTime.UtcNow.AddHours(2);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expirationDate,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
