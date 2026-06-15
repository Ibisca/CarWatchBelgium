using CarWatchBelgium.Domain.Enums;

namespace CarWatchBelgium.Application.Listings.Dto;

public class IngestListingItemDto
{
    public ListingSource Source { get; set; }
    public string? ExternalId { get; set; }
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
}
