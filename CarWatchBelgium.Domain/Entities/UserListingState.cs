namespace CarWatchBelgium.Domain.Entities;

public class UserListingState
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public Guid ListingId { get; set; }
    public bool IsSeen { get; set; }
    public bool IsFavorite { get; set; }
    public bool IsIgnored { get; set; }
    public DateTime? SeenAtUtc { get; set; }
    public DateTime? FavoritedAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public User User { get; set; } = null!;
    public Listing Listing { get; set; } = null!;
}
