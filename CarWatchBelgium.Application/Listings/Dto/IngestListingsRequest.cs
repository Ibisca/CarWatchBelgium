namespace CarWatchBelgium.Application.Listings.Dto;

public class IngestListingsRequest
{
    public Guid SavedSearchId { get; set; }
    public DateTime? ScannedAtUtc { get; set; }
    public List<IngestListingItemDto> Listings { get; set; } = new();
}
