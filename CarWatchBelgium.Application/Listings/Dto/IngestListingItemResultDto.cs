namespace CarWatchBelgium.Application.Listings.Dto;

public class IngestListingItemResultDto
{
    public Guid? ListingId { get; set; }
    public string Title { get; set; } = null!;
    public string Url { get; set; } = null!;
    public bool WasInserted { get; set; }
    public bool WasUpdated { get; set; }
    public bool WasMatched { get; set; }
    public bool WasNewMatch { get; set; }
    public bool PriceChanged { get; set; }
    public bool PriceDropped { get; set; }
    public int? OldPrice { get; set; }
    public int? NewPrice { get; set; }
    public string? SkipReason { get; set; }
}
