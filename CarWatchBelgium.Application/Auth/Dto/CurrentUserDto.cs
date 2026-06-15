namespace CarWatchBelgium.Application.Auth.Dto;

public class CurrentUserDto
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = null!;
    public string? DisplayName { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? LastLoginAtUtc { get; set; }
}
