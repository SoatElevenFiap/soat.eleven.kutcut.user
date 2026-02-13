using Soat.Eleven.Kutcut.Users.Domain.Entities;

namespace Soat.Eleven.Kutcut.Users.Domain.Interfaces.Repositories;

public interface IUserRepository
{
    Task CreateAsync(User user);
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task UpdateAsync(User user);
    Task DeleteAsync(Guid id);
}
