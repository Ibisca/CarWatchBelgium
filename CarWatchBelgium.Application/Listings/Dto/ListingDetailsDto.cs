namespace CarWatchBelgium.Application.Listings.Dto;

public class ListingDetailsDto
{
    public Guid ListingId { get; set; }
    public string Source { get; set; } = null!;
    public string? ExternalId { get; set; }
    public string Url { get; set; } = null!;
    public string? ImageUrl { get; set; }
    public string Title { get; set; } = null!;
    public string? Make { get; set; }
    public string? Model { get; set; }
    public int? Price { get; set; }
    public string Currency { get; set; } = null!;
    public int? MileageKm { get; set; }
    public int? Year { get; set; }
    public string FuelType { get; set; } = null!;
    public string Transmission { get; set; } = null!;
    public int? PowerHp { get; set; }
    public string SellerType { get; set; } = null!;
    public string? City { get; set; }
    public string CountryCode { get; set; } = null!;
    public bool IsAvailable { get; set; }
    public bool IsSeen { get; set; }
    public bool IsFavorite { get; set; }
    public bool IsIgnored { get; set; }
    public DateTime FirstSeenAtUtc { get; set; }
    public DateTime LastSeenAtUtc { get; set; }
    public DateTime? LastPriceChangeAtUtc { get; set; }
    public List<PriceHistoryDto> PriceHistory { get; set; } = new();
}
