using Soat.Eleven.Kutcut.Users.Application.DTOs.Inputs;
using Soat.Eleven.Kutcut.Users.Application.DTOs.Outputs;

namespace Soat.Eleven.Kutcut.Users.Application.Interfaces;

public interface ILoginHandle
{
    Task<LoginOutput> HandleAsync(LoginInput input);
}
