using Soat.Eleven.Kutcut.Users.Domain.Entities;
using Soat.Eleven.Kutcut.Users.Domain.Enums;

namespace Soat.Eleven.Kutcut.Users.Application.DTOs.Outputs;

public class CreateUserOutput
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public StatusUser Status { get; set; }
    public DateTime CreatedAt { get; set; }

    public static explicit operator CreateUserOutput(User user)
    {
        return new CreateUserOutput
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Status = (StatusUser)user.Status,
            CreatedAt = user.CreatedAt
        };
    }
}
