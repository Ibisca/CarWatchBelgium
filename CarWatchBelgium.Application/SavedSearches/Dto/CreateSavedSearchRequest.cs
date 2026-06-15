using CarWatchBelgium.Domain.Enums;

namespace CarWatchBelgium.Application.SavedSearches.Dto;

public class CreateSavedSearchRequest
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = null!;
    public string? Make { get; set; }
    public string? Model { get; set; }
    public string CountryCode { get; set; } = "BE";
    public int? PriceMin { get; set; }
    public int? PriceMax { get; set; }
    public int? YearMin { get; set; }
    public int? YearMax { get; set; }
    public int? MaxMileageKm { get; set; }
    public FuelType FuelType { get; set; } = FuelType.Unknown;
    public TransmissionType Transmission { get; set; } = TransmissionType.Unknown;
    public int? MinPowerHp { get; set; }
    public SellerType SellerType { get; set; } = SellerType.Unknown;
    public string? City { get; set; }
    public int? RadiusKm { get; set; }
    public List<string> RequiredKeywords { get; set; } = new();
    public List<string> ExcludedKeywords { get; set; } = new();
}
