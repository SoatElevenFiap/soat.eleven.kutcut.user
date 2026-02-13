using FluentValidation;
using Soat.Eleven.Kutcut.Users.Application.DTOs.Inputs;
using Soat.Eleven.Kutcut.Users.Application.DTOs.Outputs;
using Soat.Eleven.Kutcut.Users.Application.Interfaces;
using Soat.Eleven.Kutcut.Users.Domain.Interfaces.Repositories;
using Soat.Eleven.Kutcut.Users.Domain.Interfaces.Services;

namespace Soat.Eleven.Kutcut.Users.Application.Handlers;

public class LoginHandle : ILoginHandle
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IPasswordService _passwordService;
    private readonly IValidator<LoginInput> _loginValidator;

    public LoginHandle(IUserRepository userRepository,
                       IJwtTokenService jwtTokenService,
                       IPasswordService passwordService,
                       IValidator<LoginInput> loginValidator)
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
        _passwordService = passwordService;
        _loginValidator = loginValidator;
    }

    public async Task<LoginOutput> HandleAsync(LoginInput input)
    {
        var validationResult = await _loginValidator.ValidateAsync(input);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var passwordHash = _passwordService.TransformToHash(input.Password);

        var user = await _userRepository.GetLoginAsync(input.Email, passwordHash) ?? throw new Exception("Invalid email or password.");

        var token = _jwtTokenService.GenerateToken(user);

        return new LoginOutput(token);
    }
}
