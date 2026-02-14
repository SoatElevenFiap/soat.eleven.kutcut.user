using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Moq;
using Soat.Eleven.Kutcut.Users.Domain.Entities;
using Soat.Eleven.Kutcut.Users.Domain.Enums;
using Soat.Eleven.Kutcut.Users.Infra.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Soat.Eleven.Kutcut.Users.Tests.UnitTests.Services;

public class JwtTokenServiceTests
{
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly JwtTokenService _jwtTokenService;
    private const string SecretKey = "ThisIsAVerySecureSecretKeyForJwtTokenGeneration123456";

    public JwtTokenServiceTests()
    {
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _configurationMock = new Mock<IConfiguration>();

        _configurationMock.Setup(c => c["JwtSettings:SecretKey"]).Returns(SecretKey);

        _jwtTokenService = new JwtTokenService(
            _httpContextAccessorMock.Object,
            _configurationMock.Object
        );
    }

    [Fact]
    public void GenerateToken_GerarTokenComUsuarioValido_RetornaTokenString()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = "test@example.com",
            Password = "hashed_password",
            Status = StatusUser.Active,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var result = _jwtTokenService.GenerateToken(user);

        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public void GenerateToken_GerarTokenComUsuarioValido_TokenContemClaims()
    {
        var userId = Guid.NewGuid();
        var userEmail = "test@example.com";
        var user = new User
        {
            Id = userId,
            Name = "Test User",
            Email = userEmail,
            Password = "hashed_password",
            Status = StatusUser.Active,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var token = _jwtTokenService.GenerateToken(user);

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        var subClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);
        var emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email);
        var jtiClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti);

        Assert.NotNull(subClaim);
        Assert.Equal(userId.ToString(), subClaim.Value);
        Assert.NotNull(emailClaim);
        Assert.Equal(userEmail, emailClaim.Value);
        Assert.NotNull(jtiClaim);
        Assert.NotEmpty(jtiClaim.Value);
    }

    [Fact]
    public void GenerateToken_GerarTokenComUsuarioValido_TokenTemDataExpiracao()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = "test@example.com",
            Password = "hashed_password",
            Status = StatusUser.Active,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var token = _jwtTokenService.GenerateToken(user);

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        Assert.NotNull(jwtToken.ValidTo);
        Assert.True(jwtToken.ValidTo > DateTime.UtcNow);
        Assert.True(jwtToken.ValidTo <= DateTime.UtcNow.AddHours(2).AddMinutes(1));
    }

    [Fact]
    public void GenerateToken_GerarDoisTokensParaMesmoUsuario_RetornaTokensDiferentes()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = "test@example.com",
            Password = "hashed_password",
            Status = StatusUser.Active,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var token1 = _jwtTokenService.GenerateToken(user);
        var token2 = _jwtTokenService.GenerateToken(user);

        Assert.NotEqual(token1, token2);
    }

    [Fact]
    public void GenerateToken_GerarTokenParaUsuariosDiferentes_RetornaTokensComClaimsDiferentes()
    {
        var user1 = new User
        {
            Id = Guid.NewGuid(),
            Name = "User 1",
            Email = "user1@example.com",
            Password = "hashed_password",
            Status = StatusUser.Active,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var user2 = new User
        {
            Id = Guid.NewGuid(),
            Name = "User 2",
            Email = "user2@example.com",
            Password = "hashed_password",
            Status = StatusUser.Active,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var token1 = _jwtTokenService.GenerateToken(user1);
        var token2 = _jwtTokenService.GenerateToken(user2);

        var handler = new JwtSecurityTokenHandler();
        var jwtToken1 = handler.ReadJwtToken(token1);
        var jwtToken2 = handler.ReadJwtToken(token2);

        var sub1 = jwtToken1.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value;
        var sub2 = jwtToken2.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value;

        Assert.NotEqual(sub1, sub2);
    }

    [Fact]
    public void GetUsuarioId_ObterIdDeTokenValido_RetornaGuidDoUsuario()
    {
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            Name = "Test User",
            Email = "test@example.com",
            Password = "hashed_password",
            Status = StatusUser.Active,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var token = _jwtTokenService.GenerateToken(user);

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers.Authorization = new StringValues($"Bearer {token}");

        _httpContextAccessorMock.Setup(h => h.HttpContext).Returns(httpContext);

        var result = _jwtTokenService.GetUsuarioId();

        Assert.Equal(userId, result);
    }

    [Fact]
    public void GetUsuarioId_ObterIdSemToken_LancaAuthenticationFailureException()
    {
        var httpContext = new DefaultHttpContext();
        _httpContextAccessorMock.Setup(h => h.HttpContext).Returns(httpContext);

        Assert.Throws<AuthenticationFailureException>(() => _jwtTokenService.GetUsuarioId());
    }

    [Fact]
    public void GetUsuarioId_ObterIdComTokenInvalido_LancaException()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers.Authorization = new StringValues("Bearer invalid_token");

        _httpContextAccessorMock.Setup(h => h.HttpContext).Returns(httpContext);

        Assert.ThrowsAny<Exception>(() => _jwtTokenService.GetUsuarioId());
    }

    [Fact]
    public void GetUsuarioId_ObterIdComHttpContextNulo_LancaAuthenticationFailureException()
    {
        _httpContextAccessorMock.Setup(h => h.HttpContext).Returns((HttpContext?)null);

        Assert.Throws<AuthenticationFailureException>(() => _jwtTokenService.GetUsuarioId());
    }

    [Fact]
    public void GenerateToken_GerarTokenComEmailLongo_RetornaTokenValido()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = "very.long.email.address.for.testing.purposes@example.com",
            Password = "hashed_password",
            Status = StatusUser.Active,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var result = _jwtTokenService.GenerateToken(user);

        Assert.NotNull(result);
        Assert.NotEmpty(result);

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(result);
        var emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email);

        Assert.NotNull(emailClaim);
        Assert.Equal(user.Email, emailClaim.Value);
    }
}
