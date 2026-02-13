using FluentValidation;
using Soat.Eleven.Kutcut.Users.Application.DTOs.Inputs;
using Soat.Eleven.Kutcut.Users.Application.DTOs.Outputs;
using Soat.Eleven.Kutcut.Users.Application.Interfaces;
using Soat.Eleven.Kutcut.Users.Domain.Entities;
using Soat.Eleven.Kutcut.Users.Domain.Enums;
using Soat.Eleven.Kutcut.Users.Domain.Interfaces.Repositories;
using Soat.Eleven.Kutcut.Users.Domain.Interfaces.Services;

namespace Soat.Eleven.Kutcut.Users.Application.Handlers;

public class UserHandler : IUserHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IValidator<CreateUserInput> _createUserValidator;
    private readonly IValidator<UpdateUserInput> _updateUserValidator;
    private readonly IValidator<DeactiveUserInput> _deactiveUserValidator;
    private readonly IValidator<GetUserInput> _getUserValidator;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IPasswordService _passwordService;

    public UserHandler(IUserRepository userRepository,
                       IValidator<CreateUserInput> createUserValidator,
                       IValidator<UpdateUserInput> updateUserValidator,
                       IValidator<DeactiveUserInput> deactiveUserValidator,
                       IValidator<GetUserInput> getUserValidator,
                       IJwtTokenService jwtTokenService,
                       IPasswordService passwordService)
    {
        _userRepository = userRepository;
        _createUserValidator = createUserValidator;
        _updateUserValidator = updateUserValidator;
        _deactiveUserValidator = deactiveUserValidator;
        _getUserValidator = getUserValidator;
        _jwtTokenService = jwtTokenService;
        _passwordService = passwordService;
    }

    public async Task<CreateUserOutput> HandleAsync(CreateUserInput input)
    {
        var validationResult = await _createUserValidator.ValidateAsync(input);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var passwordHash = _passwordService.TransformToHash(input.Password);

        var user = new User
        {
            Name = input.Name,
            Email = input.Email,
            Password = passwordHash,
        };

        await _userRepository.CreateAsync(user);

        return new CreateUserOutput
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Active = user.Status,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task<UpdateUserOutput> HandleAsync(UpdateUserInput input)
    {
        input.Id = _jwtTokenService.GetUsuarioId();
        var validationResult = await _updateUserValidator.ValidateAsync(input);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var user = await _userRepository.GetByIdAsync(input.Id);
        if (user == null)
        {
            throw new Exception($"Usuário com ID {input.Id} não encontrado.");
        }

        user.Name = input.Name;
        user.Email = input.Email;

        var passwordHash = _passwordService.TransformToHash(input.Password);
        if (!user.Password.Equals(passwordHash))
        {
            user.Password = passwordHash;
        }

        await _userRepository.UpdateAsync(user);

        return new UpdateUserOutput
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Active = user.Status,
            UpdatedAt = user.UpdatedAt
        };
    }

    public async Task<DeactiveUserOutput> HandleAsync(DeactiveUserInput input)
    {
        var validationResult = await _deactiveUserValidator.ValidateAsync(input);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var user = await _userRepository.GetByIdAsync(input.Id);
        if (user == null)
        {
            throw new Exception($"Usuário com ID {input.Id} não encontrado.");
        }

        user.Status = (byte)StatusUser.Inactive;
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);

        return new DeactiveUserOutput
        {
            Id = user.Id,
            Active = user.Status,
            UpdatedAt = user.UpdatedAt,
            Message = "Usuário desativado com sucesso."
        };
    }

    public async Task<GetUserOutput> HandleAsync(GetUserInput input)
    {
        var validationResult = await _getUserValidator.ValidateAsync(input);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        User? user = null;

        if (input.Id != Guid.Empty)
        {
            user = await _userRepository.GetByIdAsync(input.Id);
        }
        else if (!string.IsNullOrEmpty(input.Email))
        {
            user = await _userRepository.GetByEmailAsync(input.Email);
        }

        if (user == null)
        {
            throw new Exception("Usuário não encontrado.");
        }

        return new GetUserOutput
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Active = user.Status,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }
}
