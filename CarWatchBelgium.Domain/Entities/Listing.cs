namespace CarWatchBelgium.Domain.Entities;

using CarWatchBelgium.Domain.Enums;

public class Listing
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public ListingSource Source { get; set; }
    public string? ExternalId { get; set; }
    public string FallbackHash { get; set; } = null!;
    public string Url { get; set; } = null!;
    public string? ImageUrl { get; set; }
    public string Title { get; set; } = null!;
    public string? Make { get; set; }
    public string? Model { get; set; }
    public int? Price { get; set; }
    public string Currency { get; set; } = "EUR";
    public int? MileageKm { get; set; }
    public int? Year { get; set; }
    public FuelType FuelType { get; set; } = FuelType.Unknown;
    public TransmissionType Transmission { get; set; } = TransmissionType.Unknown;
    public int? PowerHp { get; set; }
    public SellerType SellerType { get; set; } = SellerType.Unknown;
    public string? City { get; set; }
    public string CountryCode { get; set; } = "BE";
    public bool IsAvailable { get; set; } = true;
    public DateTime FirstSeenAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime LastSeenAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? LastPriceChangeAtUtc { get; set; }

    // Navigation properties
    public ICollection<PriceHistory> PriceHistory { get; set; } = new List<PriceHistory>();
    public ICollection<UserListingState> UserStates { get; set; } = new List<UserListingState>();
    public ICollection<SavedSearchMatch> SavedSearchMatches { get; set; } = new List<SavedSearchMatch>();
}
