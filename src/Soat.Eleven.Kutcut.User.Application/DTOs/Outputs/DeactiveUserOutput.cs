using Soat.Eleven.Kutcut.Users.Domain.Enums;

namespace Soat.Eleven.Kutcut.Users.Application.DTOs.Outputs;

public class DeactiveUserOutput
{
    public Guid Id { get; set; }
    public StatusUser Active { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string Message { get; set; }
}
