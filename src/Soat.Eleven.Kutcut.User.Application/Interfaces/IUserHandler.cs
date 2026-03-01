using Soat.Eleven.Kutcut.Users.Application.DTOs.Inputs;
using Soat.Eleven.Kutcut.Users.Application.DTOs.Outputs;

namespace Soat.Eleven.Kutcut.Users.Application.Interfaces;

public interface IUserHandler
{
    Task<CreateUserOutput> HandleAsync(CreateUserInput input);
    Task<GetUserOutput> HandleAsync(GetUserInput input);
    Task<UpdateUserOutput> HandleAsync(UpdateUserInput input);
    Task<DeactiveUserOutput> HandleAsync(DeactiveUserInput input);
}
