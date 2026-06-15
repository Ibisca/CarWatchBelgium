namespace CarWatchBelgium.Application.SavedSearches.Dto;

public class CreateSavedSearchForCurrentUserRequest
{
    public string Name { get; set; } = null!;
    public string? Make { get; set; }
    public string? Model { get; set; }
    public string CountryCode { get; set; } = "BE";
    public int? PriceMin { get; set; }
    public int? PriceMax { get; set; }
    public int? YearMin { get; set; }
    public int? YearMax { get; set; }
    public int? MaxMileageKm { get; set; }
    public CarWatchBelgium.Domain.Enums.FuelType FuelType { get; set; } = CarWatchBelgium.Domain.Enums.FuelType.Unknown;
    public CarWatchBelgium.Domain.Enums.TransmissionType Transmission { get; set; } = CarWatchBelgium.Domain.Enums.TransmissionType.Unknown;
    public int? MinPowerHp { get; set; }
    public CarWatchBelgium.Domain.Enums.SellerType SellerType { get; set; } = CarWatchBelgium.Domain.Enums.SellerType.Unknown;
    public string? City { get; set; }
    public int? RadiusKm { get; set; }
    public List<string> RequiredKeywords { get; set; } = new();
    public List<string> ExcludedKeywords { get; set; } = new();
}
