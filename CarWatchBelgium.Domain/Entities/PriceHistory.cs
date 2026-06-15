namespace CarWatchBelgium.Domain.Entities;

public class PriceHistory
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ListingId { get; set; }
    public int Price { get; set; }
    public string Currency { get; set; } = "EUR";
    public DateTime CapturedAtUtc { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Listing Listing { get; set; } = null!;
}
