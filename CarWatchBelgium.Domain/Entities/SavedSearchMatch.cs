namespace CarWatchBelgium.Domain.Entities;

public class SavedSearchMatch
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SavedSearchId { get; set; }
    public Guid ListingId { get; set; }
    public DateTime MatchedAtUtc { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public SavedSearch SavedSearch { get; set; } = null!;
    public Listing Listing { get; set; } = null!;
}
