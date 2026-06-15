namespace CarWatchBelgium.Application.Listings.Dto;

public class IngestListingsResult
{
    public int ReceivedCount { get; set; }
    public int InsertedCount { get; set; }
    public int UpdatedCount { get; set; }
    public int MatchedCount { get; set; }
    public int NewMatchCount { get; set; }
    public int PriceChangedCount { get; set; }
    public int PriceDroppedCount { get; set; }
    public int SkippedCount { get; set; }
    public List<IngestListingItemResultDto> Items { get; set; } = new();
}
