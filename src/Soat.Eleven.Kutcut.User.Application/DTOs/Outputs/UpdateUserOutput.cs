using Soat.Eleven.Kutcut.Users.Domain.Enums;

namespace Soat.Eleven.Kutcut.Users.Application.DTOs.Outputs;

public class UpdateUserOutput
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public StatusUser Status { get; set; }
    public DateTime UpdatedAt { get; set; }

    public static explicit operator UpdateUserOutput(Users.Domain.Entities.User user)
    {
        return new UpdateUserOutput
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Status = (StatusUser)user.Status,
            UpdatedAt = user.UpdatedAt
        };
    }
}
