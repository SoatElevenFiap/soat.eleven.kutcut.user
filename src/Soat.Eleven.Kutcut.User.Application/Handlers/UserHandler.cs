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
    private readonly ICacheService _cacheService;

    public UserHandler(IUserRepository userRepository,
                       IValidator<CreateUserInput> createUserValidator,
                       IValidator<UpdateUserInput> updateUserValidator,
                       IValidator<DeactiveUserInput> deactiveUserValidator,
                       IValidator<GetUserInput> getUserValidator,
                       IJwtTokenService jwtTokenService,
                       IPasswordService passwordService,
                       ICacheService cacheService)
    {
        _userRepository = userRepository;
        _createUserValidator = createUserValidator;
        _updateUserValidator = updateUserValidator;
        _deactiveUserValidator = deactiveUserValidator;
        _getUserValidator = getUserValidator;
        _jwtTokenService = jwtTokenService;
        _passwordService = passwordService;
        _cacheService = cacheService;
    }

    public async Task<CreateUserOutput> HandleAsync(CreateUserInput input)
    {
        var validationResult = await _createUserValidator.ValidateAsync(input);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var userExisting = await _userRepository.GetByEmailAsync(input.Email);
        if (userExisting is not null)
            throw new Exception("Usuário com este email já existe.");

        var passwordHash = _passwordService.TransformToHash(input.Password);

        var user = new User
        {
            Name = input.Name,
            Email = input.Email,
            Password = passwordHash,
        };

        await _userRepository.CreateAsync(user);
        await _cacheService.SetAsync(GetCacheUserKey(user.Id), user);

        return (CreateUserOutput)user;
    }

    public async Task<UpdateUserOutput> HandleAsync(UpdateUserInput input)
    {
        input.Id = _jwtTokenService.GetUsuarioId();

        var validationResult = await _updateUserValidator.ValidateAsync(input);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var user = await GetUser(input.Id) ?? throw new Exception($"Usuário com ID {input.Id} não encontrado.");

        if (input.Email != user.Email)
        {
            var userExisting = await _userRepository.GetByEmailAsync(input.Email);
            if (userExisting is not null)
                throw new Exception("Usuário com este email já existe.");
        }

        user.Name = input.Name;
        user.Email = input.Email;

        var passwordHash = _passwordService.TransformToHash(input.Password);
        if (!user.Password.Equals(passwordHash))
            user.Password = passwordHash;

        await _userRepository.UpdateAsync(user);
        await _cacheService.SetAsync(GetCacheUserKey(user.Id), user);

        return (UpdateUserOutput)user;
    }

    public async Task<DeactiveUserOutput> HandleAsync(DeactiveUserInput input)
    {
        var validationResult = await _deactiveUserValidator.ValidateAsync(input);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var user = (await _cacheService.GetAsync<User>(GetCacheUserKey(input.Id)) ?? await _userRepository.GetByIdAsync(input.Id))
            ?? throw new Exception($"Usuário com ID {input.Id} não encontrado.");

        user.Status = (byte)StatusUser.Inactive;
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);
        await _cacheService.SetAsync(GetCacheUserKey(user.Id), user);

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
            throw new ValidationException(validationResult.Errors);

        User? user = null;

        if (input.Id != Guid.Empty)
        {
            user = await GetUser(input.Id);
        }
        else if (!string.IsNullOrEmpty(input.Email))
        {
            user = await _userRepository.GetByEmailAsync(input.Email);
        }

        if (user == null)
            throw new Exception("Usuário não encontrado.");

        return (GetUserOutput)user;
    }

    private async Task<User?> GetUser(Guid userId)
    {
        var user = await _cacheService.GetAsync<User>(GetCacheUserKey(userId));

        if (user is not null)
            return user;

        user = await _userRepository.GetByIdAsync(userId);

        if (user is not null)
            await _cacheService.SetAsync(GetCacheUserKey(user.Id), user);

        return user;
    }

    private string GetCacheUserKey(Guid userId) => $"user:{userId}";
}
