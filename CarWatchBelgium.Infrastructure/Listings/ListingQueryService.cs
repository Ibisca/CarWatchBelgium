using Microsoft.EntityFrameworkCore;
using CarWatchBelgium.Application.Listings.Dto;
using CarWatchBelgium.Application.Listings.Services;
using CarWatchBelgium.Domain.Entities;
using CarWatchBelgium.Infrastructure.Persistence;

namespace CarWatchBelgium.Infrastructure.Listings;

public class ListingQueryService : IListingQueryService
{
    private readonly AppDbContext _context;

    public ListingQueryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<MatchedListingDto>> GetBySavedSearchAsync(Guid savedSearchId, Guid userId, bool includeUnavailable = false, bool includeIgnored = false, CancellationToken cancellationToken = default)
    {
        // Verifică dacă SavedSearch există
        var savedSearchExists = await _context.SavedSearches
            .AsNoTracking()
            .AnyAsync(ss => ss.Id == savedSearchId, cancellationToken);

        if (!savedSearchExists)
        {
            throw new KeyNotFoundException($"SavedSearch with ID {savedSearchId} does not exist.");
        }

        // Citește anunțurile prin SavedSearchMatches
        var matches = await _context.SavedSearchMatches
            .AsNoTracking()
            .Where(m => m.SavedSearchId == savedSearchId)
            .Include(m => m.Listing!)
                .ThenInclude(l => l.PriceHistory)
            .Include(m => m.Listing!)
                .ThenInclude(l => l.UserStates)
            .ToListAsync(cancellationToken);

        var result = new List<MatchedListingDto>();

        foreach (var match in matches)
        {
            var listing = match.Listing;
            if (listing == null)
                continue;

            // Exclude dacă nu e available
            if (!includeUnavailable && !listing.IsAvailable)
                continue;

            // Cauta UserListingState pentru UserId + ListingId
            var userState = listing.UserStates.FirstOrDefault(us => us.UserId == userId);

            // Exclude dacă e ignorat și includeIgnored = false
            if (!includeIgnored && userState?.IsIgnored == true)
                continue;

            var dto = ToMatchedListingDto(listing, match, userState);
            result.Add(dto);
        }

        // Ordonează:
        // 1. anunțuri nevăzute primele
        // 2. apoi price drop
        // 3. apoi MatchedAtUtc descrescător
        result = result
            .OrderBy(m => m.IsSeen)
            .ThenByDescending(m => m.PriceDropped)
            .ThenByDescending(m => m.MatchedAtUtc)
            .ToList();

        return result;
    }

    public async Task<List<ListingDetailsDto>> GetFavoritesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var listings = await _context.Listings
            .AsNoTracking()
            .Include(l => l.UserStates)
            .Include(l => l.PriceHistory)
            .Where(l => l.UserStates.Any(us => us.UserId == userId && us.IsFavorite && !us.IsIgnored))
            .OrderByDescending(l => l.UserStates
                .Where(us => us.UserId == userId)
                .Select(us => us.FavoritedAtUtc)
                .FirstOrDefault())
            .ToListAsync(cancellationToken);

        var result = new List<ListingDetailsDto>();

        foreach (var listing in listings)
        {
            var userState = listing.UserStates.FirstOrDefault(us => us.UserId == userId);
            var dto = ToListingDetailsDto(listing, userState);
            result.Add(dto);
        }

        return result;
    }

    public async Task<List<MatchedListingDto>> GetUnseenAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var listings = await _context.Listings
            .AsNoTracking()
            .Include(l => l.UserStates)
            .Include(l => l.PriceHistory)
            .Include(l => l.SavedSearchMatches)
            .Where(l =>
                l.IsAvailable &&
                (
                    !l.UserStates.Any(us => us.UserId == userId && us.IsSeen) &&
                    !l.UserStates.Any(us => us.UserId == userId && us.IsIgnored)
                ) ||
                (
                    l.UserStates.Any(us => us.UserId == userId && !us.IsSeen && !us.IsIgnored)
                )
            )
            .OrderByDescending(l => l.FirstSeenAtUtc)
            .ToListAsync(cancellationToken);

        var result = new List<MatchedListingDto>();

        foreach (var listing in listings)
        {
            var userState = listing.UserStates.FirstOrDefault(us => us.UserId == userId);
            
            // Get any match - preferably the most recent one
            var match = listing.SavedSearchMatches.OrderByDescending(m => m.MatchedAtUtc).FirstOrDefault();
            
            if (match != null)
            {
                var dto = ToMatchedListingDto(listing, match, userState);
                result.Add(dto);
            }
        }

        return result;
    }

    public async Task<ListingDetailsDto?> GetDetailsAsync(Guid listingId, Guid userId, CancellationToken cancellationToken = default)
    {
        var listing = await _context.Listings
            .AsNoTracking()
            .Include(l => l.UserStates)
            .Include(l => l.PriceHistory)
            .FirstOrDefaultAsync(l => l.Id == listingId, cancellationToken);

        if (listing == null)
            return null;

        var userState = listing.UserStates.FirstOrDefault(us => us.UserId == userId);
        return ToListingDetailsDto(listing, userState);
    }

    private static MatchedListingDto ToMatchedListingDto(Listing listing, SavedSearchMatch match, UserListingState? userState)
    {
        // Calculează PreviousPrice din ultimele două prețuri
        int? previousPrice = null;
        bool priceChanged = listing.LastPriceChangeAtUtc != null;
        bool priceDropped = false;
        int? priceDropAmount = null;

        if (listing.PriceHistory.Count >= 2 && priceChanged)
        {
            var sortedPrices = listing.PriceHistory
                .OrderByDescending(ph => ph.CapturedAtUtc)
                .ToList();
            
            previousPrice = sortedPrices.Count > 1 ? sortedPrices[1].Price : null;
            
            if (previousPrice.HasValue && listing.Price.HasValue && listing.Price.Value < previousPrice.Value)
            {
                priceDropped = true;
                priceDropAmount = previousPrice.Value - listing.Price.Value;
            }
        }

        return new MatchedListingDto
        {
            ListingId = listing.Id,
            SavedSearchId = match.SavedSearchId,
            UserListingStateId = userState?.Id,
            Source = listing.Source.ToString(),
            ExternalId = listing.ExternalId,
            Url = listing.Url,
            ImageUrl = listing.ImageUrl,
            Title = listing.Title,
            Make = listing.Make,
            Model = listing.Model,
            Price = listing.Price,
            Currency = listing.Currency,
            PreviousPrice = previousPrice,
            PriceChanged = priceChanged,
            PriceDropped = priceDropped,
            PriceDropAmount = priceDropAmount,
            MileageKm = listing.MileageKm,
            Year = listing.Year,
            FuelType = listing.FuelType.ToString(),
            Transmission = listing.Transmission.ToString(),
            PowerHp = listing.PowerHp,
            SellerType = listing.SellerType.ToString(),
            City = listing.City,
            CountryCode = listing.CountryCode,
            IsAvailable = listing.IsAvailable,
            IsSeen = userState?.IsSeen ?? false,
            IsFavorite = userState?.IsFavorite ?? false,
            IsIgnored = userState?.IsIgnored ?? false,
            IsNew = !(userState?.IsSeen ?? false),
            FirstSeenAtUtc = listing.FirstSeenAtUtc,
            LastSeenAtUtc = listing.LastSeenAtUtc,
            LastPriceChangeAtUtc = listing.LastPriceChangeAtUtc,
            MatchedAtUtc = match.MatchedAtUtc
        };
    }

    private static ListingDetailsDto ToListingDetailsDto(Listing listing, UserListingState? userState)
    {
        var priceHistory = listing.PriceHistory
            .OrderByDescending(ph => ph.CapturedAtUtc)
            .Select(ph => new PriceHistoryDto
            {
                Id = ph.Id,
                Price = ph.Price,
                Currency = ph.Currency,
                CapturedAtUtc = ph.CapturedAtUtc
            })
            .ToList();

        return new ListingDetailsDto
        {
            ListingId = listing.Id,
            Source = listing.Source.ToString(),
            ExternalId = listing.ExternalId,
            Url = listing.Url,
            ImageUrl = listing.ImageUrl,
            Title = listing.Title,
            Make = listing.Make,
            Model = listing.Model,
            Price = listing.Price,
            Currency = listing.Currency,
            MileageKm = listing.MileageKm,
            Year = listing.Year,
            FuelType = listing.FuelType.ToString(),
            Transmission = listing.Transmission.ToString(),
            PowerHp = listing.PowerHp,
            SellerType = listing.SellerType.ToString(),
            City = listing.City,
            CountryCode = listing.CountryCode,
            IsAvailable = listing.IsAvailable,
            IsSeen = userState?.IsSeen ?? false,
            IsFavorite = userState?.IsFavorite ?? false,
            IsIgnored = userState?.IsIgnored ?? false,
            FirstSeenAtUtc = listing.FirstSeenAtUtc,
            LastSeenAtUtc = listing.LastSeenAtUtc,
            LastPriceChangeAtUtc = listing.LastPriceChangeAtUtc,
            PriceHistory = priceHistory
        };
    }
}
