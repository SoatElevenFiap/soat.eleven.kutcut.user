using Microsoft.EntityFrameworkCore;
using Soat.Eleven.Kutcut.Users.Domain.Entities;
using Soat.Eleven.Kutcut.Users.Domain.Enums;
using Soat.Eleven.Kutcut.Users.Domain.Interfaces.Repositories;
using Soat.Eleven.Kutcut.Users.Infra.Context;

namespace Soat.Eleven.Kutcut.Users.Infra.Repositories;

public class UserRepository(UserDataContext context) : IUserRepository
{
    public async Task CreateAsync(User user)
    {
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var user = await GetByIdAsync(id);
        if (user != null)
        {
            context.Users.Remove(user);
            await context.SaveChangesAsync();
        }
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await context.Users.FindAsync(id);
    }

    public async Task<User?> GetLoginAsync(string email, string password)
    {
        return await context.Users.FirstOrDefaultAsync(u => u.Email == email && u.Password == password && u.Status == StatusUser.Active);
    }

    public async Task UpdateAsync(User user)
    {
        context.Users.Update(user);
        await context.SaveChangesAsync();
    }
}
