namespace Soat.Eleven.Kutcut.Users.Domain.Interfaces.Services;

public interface IPasswordService
{
    string TransformToHash(string password);
    bool Verify(string password, string hash);
}
