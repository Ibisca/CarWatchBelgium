namespace CarWatchBelgium.Application.Listings.Dto;

public class PriceHistoryDto
{
    public Guid Id { get; set; }
    public int Price { get; set; }
    public string Currency { get; set; } = null!;
    public DateTime CapturedAtUtc { get; set; }
}
