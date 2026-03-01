using Microsoft.Extensions.Configuration;
using Soat.Eleven.Kutcut.Users.Domain.Interfaces.Services;
using System.Security.Cryptography;
using System.Text;

namespace Soat.Eleven.Kutcut.Users.Infra.Services;

public class PasswordService : IPasswordService
{
    private readonly string _saltKey;

    public PasswordService(IConfiguration configuration)
    {
        _saltKey = configuration["SaltKey"]!;
    }
    public string TransformToHash(string password)
    {
        var saltByte = Encoding.UTF8.GetBytes(_saltKey);
        var hmacMD5 = new HMACMD5(saltByte);
        var passwordConvert = Encoding.UTF8.GetBytes(password!);
        return Convert.ToBase64String(hmacMD5.ComputeHash(passwordConvert));
    }

    public bool Verify(string password, string hash)
    {
        var newPasswordHash = TransformToHash(password);
        return newPasswordHash == hash;
    }
}
