using Microsoft.EntityFrameworkCore;
using CarWatchBelgium.Application.SavedSearches.Dto;
using CarWatchBelgium.Application.SavedSearches.Services;
using CarWatchBelgium.Domain.Entities;
using CarWatchBelgium.Infrastructure.Persistence;

namespace CarWatchBelgium.Infrastructure.SavedSearches;

public class SavedSearchService : ISavedSearchService
{
    private readonly AppDbContext _context;

    public SavedSearchService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<SavedSearchDto> CreateAsync(CreateSavedSearchRequest request, CancellationToken cancellationToken = default)
    {
        // A. Validări
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("Name is required and cannot be empty.", nameof(request.Name));
        }

        var userExists = await _context.Users
            .AsNoTracking()
            .AnyAsync(u => u.Id == request.UserId, cancellationToken);

        if (!userExists)
        {
            throw new KeyNotFoundException($"User with ID {request.UserId} does not exist.");
        }

        // B. Normalizare
        var name = request.Name.Trim();
        var make = string.IsNullOrWhiteSpace(request.Make) ? null : request.Make.Trim();
        var model = string.IsNullOrWhiteSpace(request.Model) ? null : request.Model.Trim();
        var countryCode = string.IsNullOrWhiteSpace(request.CountryCode) ? "BE" : request.CountryCode.ToUpperInvariant();
        var city = string.IsNullOrWhiteSpace(request.City) ? null : request.City.Trim();
        var requiredKeywords = NormalizeKeywords(request.RequiredKeywords);
        var excludedKeywords = NormalizeKeywords(request.ExcludedKeywords);

        // C. Creează SavedSearch
        var savedSearch = new SavedSearch
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            Name = name,
            Make = make,
            Model = model,
            CountryCode = countryCode,
            PriceMin = request.PriceMin,
            PriceMax = request.PriceMax,
            YearMin = request.YearMin,
            YearMax = request.YearMax,
            MaxMileageKm = request.MaxMileageKm,
            FuelType = request.FuelType,
            Transmission = request.Transmission,
            MinPowerHp = request.MinPowerHp,
            SellerType = request.SellerType,
            City = city,
            RadiusKm = request.RadiusKm,
            RequiredKeywords = requiredKeywords,
            ExcludedKeywords = excludedKeywords,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow,
            LastMatchedAtUtc = null
        };

        _context.SavedSearches.Add(savedSearch);
        await _context.SaveChangesAsync(cancellationToken);

        return ToDto(savedSearch, 0);
    }

    public async Task<List<SavedSearchDto>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var savedSearches = await _context.SavedSearches
            .AsNoTracking()
            .Where(ss => ss.UserId == userId)
            .OrderByDescending(ss => ss.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        var result = new List<SavedSearchDto>();

        foreach (var ss in savedSearches)
        {
            var matchCount = await _context.SavedSearchMatches
                .AsNoTracking()
                .CountAsync(m => m.SavedSearchId == ss.Id, cancellationToken);

            result.Add(ToDto(ss, matchCount));
        }

        return result;
    }

    public async Task<SavedSearchDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var savedSearch = await _context.SavedSearches
            .AsNoTracking()
            .FirstOrDefaultAsync(ss => ss.Id == id, cancellationToken);

        if (savedSearch == null)
        {
            return null;
        }

        var matchCount = await _context.SavedSearchMatches
            .AsNoTracking()
            .CountAsync(m => m.SavedSearchId == id, cancellationToken);

        return ToDto(savedSearch, matchCount);
    }

    public async Task<SavedSearchDto?> UpdateAsync(Guid id, UpdateSavedSearchRequest request, CancellationToken cancellationToken = default)
    {
        var savedSearch = await _context.SavedSearches
            .AsTracking()
            .FirstOrDefaultAsync(ss => ss.Id == id, cancellationToken);

        if (savedSearch == null)
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("Name is required and cannot be empty.", nameof(request.Name));
        }

        // Normalizare
        var name = request.Name.Trim();
        var make = string.IsNullOrWhiteSpace(request.Make) ? null : request.Make.Trim();
        var model = string.IsNullOrWhiteSpace(request.Model) ? null : request.Model.Trim();
        var countryCode = string.IsNullOrWhiteSpace(request.CountryCode) ? "BE" : request.CountryCode.ToUpperInvariant();
        var city = string.IsNullOrWhiteSpace(request.City) ? null : request.City.Trim();
        var requiredKeywords = NormalizeKeywords(request.RequiredKeywords);
        var excludedKeywords = NormalizeKeywords(request.ExcludedKeywords);

        // Actualizare
        savedSearch.Name = name;
        savedSearch.Make = make;
        savedSearch.Model = model;
        savedSearch.CountryCode = countryCode;
        savedSearch.PriceMin = request.PriceMin;
        savedSearch.PriceMax = request.PriceMax;
        savedSearch.YearMin = request.YearMin;
        savedSearch.YearMax = request.YearMax;
        savedSearch.MaxMileageKm = request.MaxMileageKm;
        savedSearch.FuelType = request.FuelType;
        savedSearch.Transmission = request.Transmission;
        savedSearch.MinPowerHp = request.MinPowerHp;
        savedSearch.SellerType = request.SellerType;
        savedSearch.City = city;
        savedSearch.RadiusKm = request.RadiusKm;
        savedSearch.RequiredKeywords = requiredKeywords;
        savedSearch.ExcludedKeywords = excludedKeywords;
        savedSearch.IsActive = request.IsActive;

        await _context.SaveChangesAsync(cancellationToken);

        var matchCount = await _context.SavedSearchMatches
            .AsNoTracking()
            .CountAsync(m => m.SavedSearchId == id, cancellationToken);

        return ToDto(savedSearch, matchCount);
    }

    public async Task<bool> DeactivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var savedSearch = await _context.SavedSearches
            .AsTracking()
            .FirstOrDefaultAsync(ss => ss.Id == id, cancellationToken);

        if (savedSearch == null)
        {
            return false;
        }

        savedSearch.IsActive = false;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var savedSearch = await _context.SavedSearches
            .AsTracking()
            .FirstOrDefaultAsync(ss => ss.Id == id, cancellationToken);

        if (savedSearch == null)
        {
            return false;
        }

        _context.SavedSearches.Remove(savedSearch);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static List<string> NormalizeKeywords(List<string>? keywords)
    {
        if (keywords == null || keywords.Count == 0)
        {
            return new List<string>();
        }

        return keywords
            .Where(k => !string.IsNullOrWhiteSpace(k))
            .Select(k => k.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static SavedSearchDto ToDto(SavedSearch savedSearch, int matchCount)
    {
        return new SavedSearchDto
        {
            Id = savedSearch.Id,
            UserId = savedSearch.UserId,
            Name = savedSearch.Name,
            Make = savedSearch.Make,
            Model = savedSearch.Model,
            CountryCode = savedSearch.CountryCode,
            PriceMin = savedSearch.PriceMin,
            PriceMax = savedSearch.PriceMax,
            YearMin = savedSearch.YearMin,
            YearMax = savedSearch.YearMax,
            MaxMileageKm = savedSearch.MaxMileageKm,
            FuelType = savedSearch.FuelType,
            Transmission = savedSearch.Transmission,
            MinPowerHp = savedSearch.MinPowerHp,
            SellerType = savedSearch.SellerType,
            City = savedSearch.City,
            RadiusKm = savedSearch.RadiusKm,
            RequiredKeywords = savedSearch.RequiredKeywords,
            ExcludedKeywords = savedSearch.ExcludedKeywords,
            IsActive = savedSearch.IsActive,
            CreatedAtUtc = savedSearch.CreatedAtUtc,
            LastMatchedAtUtc = savedSearch.LastMatchedAtUtc,
            MatchCount = matchCount
        };
    }
}
