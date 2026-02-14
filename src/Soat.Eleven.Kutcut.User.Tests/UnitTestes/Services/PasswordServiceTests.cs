using Microsoft.Extensions.Configuration;
using Moq;
using Soat.Eleven.Kutcut.Users.Infra.Services;

namespace Soat.Eleven.Kutcut.Users.Tests.UnitTests.Services;

public class PasswordServiceTests
{
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly PasswordService _passwordService;
    private const string SaltKey = "TestSaltKey123!@#";

    public PasswordServiceTests()
    {
        _configurationMock = new Mock<IConfiguration>();
        _configurationMock.Setup(c => c["SaltKey"]).Returns(SaltKey);

        _passwordService = new PasswordService(_configurationMock.Object);
    }

    [Fact]
    public void TransformToHash_ConverterSenhaParaHash_RetornaHashString()
    {
        var password = "SecurePassword123";

        var result = _passwordService.TransformToHash(password);

        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.NotEqual(password, result);
    }

    [Fact]
    public void TransformToHash_ConverterMesmaSenhaDuasVezes_RetornaMesmoHash()
    {
        var password = "SecurePassword123";

        var hash1 = _passwordService.TransformToHash(password);
        var hash2 = _passwordService.TransformToHash(password);

        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void TransformToHash_ConverterSenhasDiferentes_RetornaHashesDiferentes()
    {
        var password1 = "Password123";
        var password2 = "Password456";

        var hash1 = _passwordService.TransformToHash(password1);
        var hash2 = _passwordService.TransformToHash(password2);

        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void TransformToHash_ConverterSenhaVazia_RetornaHash()
    {
        var password = "";

        var result = _passwordService.TransformToHash(password);

        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public void Verify_VerificarSenhaCorretaComHash_RetornaTrue()
    {
        var password = "SecurePassword123";
        var hash = _passwordService.TransformToHash(password);

        var result = _passwordService.Verify(password, hash);

        Assert.True(result);
    }

    [Fact]
    public void Verify_VerificarSenhaIncorretaComHash_RetornaFalse()
    {
        var correctPassword = "SecurePassword123";
        var incorrectPassword = "WrongPassword456";
        var hash = _passwordService.TransformToHash(correctPassword);

        var result = _passwordService.Verify(incorrectPassword, hash);

        Assert.False(result);
    }

    [Fact]
    public void Verify_VerificarSenhaVaziaComHash_RetornaFalse()
    {
        var password = "SecurePassword123";
        var hash = _passwordService.TransformToHash(password);

        var result = _passwordService.Verify("", hash);

        Assert.False(result);
    }

    [Fact]
    public void Verify_VerificarSenhaComHashVazio_RetornaFalse()
    {
        var password = "SecurePassword123";

        var result = _passwordService.Verify(password, "");

        Assert.False(result);
    }

    [Fact]
    public void Verify_VerificarSenhaComCaseSensitive_RetornaFalse()
    {
        var password = "SecurePassword123";
        var hash = _passwordService.TransformToHash(password);

        var result = _passwordService.Verify("securepassword123", hash);

        Assert.False(result);
    }

    [Fact]
    public void TransformToHash_ConverterSenhaComCaracteresEspeciais_RetornaHash()
    {
        var password = "P@ssw0rd!#$%&*()";

        var result = _passwordService.TransformToHash(password);

        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.NotEqual(password, result);
    }

    [Fact]
    public void Verify_VerificarSenhaComEspacos_RetornaResultadoCorreto()
    {
        var password = "Password With Spaces";
        var hash = _passwordService.TransformToHash(password);

        var resultCorrect = _passwordService.Verify(password, hash);
        var resultIncorrect = _passwordService.Verify("PasswordWithSpaces", hash);

        Assert.True(resultCorrect);
        Assert.False(resultIncorrect);
    }

    [Fact]
    public void TransformToHash_ConverterSenhaLonga_RetornaHash()
    {
        var password = new string('a', 1000);

        var result = _passwordService.TransformToHash(password);

        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }
}
