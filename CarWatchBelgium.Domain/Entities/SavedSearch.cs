namespace CarWatchBelgium.Domain.Entities;

using CarWatchBelgium.Domain.Enums;

public class SavedSearch
{
    public Guid Id { get; set; } = Guid.NewGuid();
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
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? LastMatchedAtUtc { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public ICollection<SavedSearchMatch> Matches { get; set; } = new List<SavedSearchMatch>();
}
