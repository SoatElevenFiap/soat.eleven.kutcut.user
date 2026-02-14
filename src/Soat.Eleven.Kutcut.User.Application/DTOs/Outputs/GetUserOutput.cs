using Soat.Eleven.Kutcut.Users.Domain.Entities;
using Soat.Eleven.Kutcut.Users.Domain.Enums;

namespace Soat.Eleven.Kutcut.Users.Application.DTOs.Outputs;

public class GetUserOutput
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public StatusUser Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public static explicit operator GetUserOutput(User user)
        {
            return new GetUserOutput
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Status = (StatusUser)user.Status,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
    }
}
