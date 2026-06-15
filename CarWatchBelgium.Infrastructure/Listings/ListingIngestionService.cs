using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using CarWatchBelgium.Application.Listings.Dto;
using CarWatchBelgium.Application.Listings.Services;
using CarWatchBelgium.Domain.Entities;
using CarWatchBelgium.Infrastructure.Persistence;

namespace CarWatchBelgium.Infrastructure.Listings;

public class ListingIngestionService : IListingIngestionService
{
    private readonly AppDbContext _context;

    public ListingIngestionService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IngestListingsResult> IngestAsync(IngestListingsRequest request, CancellationToken cancellationToken = default)
    {
        var result = new IngestListingsResult
        {
            ReceivedCount = request.Listings.Count
        };

        var scannedAtUtc = request.ScannedAtUtc ?? DateTime.UtcNow;

        // Validează SavedSearchId
        var savedSearch = await _context.SavedSearches
            .AsNoTracking()
            .FirstOrDefaultAsync(ss => ss.Id == request.SavedSearchId, cancellationToken);

        if (savedSearch == null)
        {
            throw new InvalidOperationException($"SavedSearch with ID {request.SavedSearchId} does not exist.");
        }

        var hasValidListings = false;

        foreach (var item in request.Listings)
        {
            var itemResult = new IngestListingItemResultDto
            {
                Title = item.Title ?? "N/A",
                Url = item.Url ?? "N/A"
            };

            try
            {
                // A. Validări minime
                if (string.IsNullOrWhiteSpace(item.Url))
                {
                    itemResult.SkipReason = "Missing URL";
                    result.SkippedCount++;
                    result.Items.Add(itemResult);
                    continue;
                }

                if (string.IsNullOrWhiteSpace(item.Title))
                {
                    itemResult.SkipReason = "Missing title";
                    result.SkippedCount++;
                    result.Items.Add(itemResult);
                    continue;
                }

                hasValidListings = true;

                // B. Normalizează datele
                var currency = string.IsNullOrWhiteSpace(item.Currency) ? "EUR" : item.Currency;
                var countryCode = string.IsNullOrWhiteSpace(item.CountryCode) ? "BE" : item.CountryCode;
                var externalId = string.IsNullOrWhiteSpace(item.ExternalId) ? null : item.ExternalId.Trim();
                var url = item.Url.Trim();
                var title = item.Title.Trim();

                // C. Calculează FallbackHash
                var fallbackHash = ComputeFallbackHash(title, item.Price, url);

                // D. Căutare listing existent
                Listing? existingListing = null;

                if (!string.IsNullOrEmpty(externalId))
                {
                    existingListing = await _context.Listings
                        .AsTracking()
                        .FirstOrDefaultAsync(l => l.Source == item.Source && l.ExternalId == externalId, cancellationToken);
                }
                else
                {
                    existingListing = await _context.Listings
                        .AsTracking()
                        .FirstOrDefaultAsync(l => l.Source == item.Source && l.FallbackHash == fallbackHash, cancellationToken);
                }

                int? oldPrice = null;
                bool priceChanged = false;
                bool priceDropped = false;

                if (existingListing == null)
                {
                    // F. Creează Listing nou
                    var newListing = new Listing
                    {
                        Id = Guid.NewGuid(),
                        Source = item.Source,
                        ExternalId = externalId,
                        FallbackHash = fallbackHash,
                        Url = url,
                        ImageUrl = item.ImageUrl,
                        Title = title,
                        Make = item.Make,
                        Model = item.Model,
                        Price = item.Price,
                        Currency = currency,
                        MileageKm = item.MileageKm,
                        Year = item.Year,
                        FuelType = item.FuelType,
                        Transmission = item.Transmission,
                        PowerHp = item.PowerHp,
                        SellerType = item.SellerType,
                        City = item.City,
                        CountryCode = countryCode,
                        IsAvailable = true,
                        FirstSeenAtUtc = scannedAtUtc,
                        LastSeenAtUtc = scannedAtUtc,
                        LastPriceChangeAtUtc = null
                    };

                    _context.Listings.Add(newListing);
                    await _context.SaveChangesAsync(cancellationToken);

                    itemResult.ListingId = newListing.Id;
                    itemResult.WasInserted = true;
                    result.InsertedCount++;

                    // Adaugă PriceHistory inițial dacă Price are valoare
                    if (item.Price.HasValue)
                    {
                        var priceHistory = new PriceHistory
                        {
                            Id = Guid.NewGuid(),
                            ListingId = newListing.Id,
                            Price = item.Price.Value,
                            Currency = currency,
                            CapturedAtUtc = scannedAtUtc
                        };
                        _context.PriceHistories.Add(priceHistory);
                    }

                    existingListing = newListing;
                }
                else
                {
                    // G. Actualizează Listing existent
                    oldPrice = existingListing.Price;

                    existingListing.LastSeenAtUtc = scannedAtUtc;
                    existingListing.IsAvailable = true;
                    existingListing.Url = url;
                    existingListing.ImageUrl = item.ImageUrl;
                    existingListing.Title = title;
                    existingListing.Make = item.Make;
                    existingListing.Model = item.Model;
                    existingListing.MileageKm = item.MileageKm;
                    existingListing.Year = item.Year;
                    existingListing.FuelType = item.FuelType;
                    existingListing.Transmission = item.Transmission;
                    existingListing.PowerHp = item.PowerHp;
                    existingListing.SellerType = item.SellerType;
                    existingListing.City = item.City;
                    existingListing.CountryCode = countryCode;
                    existingListing.Currency = currency;

                    // Verifică schimbarea de preț
                    if (item.Price.HasValue && existingListing.Price != item.Price.Value)
                    {
                        var newPriceHistory = new PriceHistory
                        {
                            Id = Guid.NewGuid(),
                            ListingId = existingListing.Id,
                            Price = item.Price.Value,
                            Currency = currency,
                            CapturedAtUtc = scannedAtUtc
                        };
                        _context.PriceHistories.Add(newPriceHistory);

                        existingListing.LastPriceChangeAtUtc = scannedAtUtc;
                        existingListing.Price = item.Price.Value;

                        priceChanged = true;
                        result.PriceChangedCount++;

                        if (oldPrice.HasValue && item.Price.Value < oldPrice.Value)
                        {
                            priceDropped = true;
                            result.PriceDroppedCount++;
                        }
                    }

                    itemResult.WasUpdated = true;
                    result.UpdatedCount++;
                }

                itemResult.ListingId = existingListing.Id;

                // H. SavedSearchMatch
                var existingMatch = await _context.SavedSearchMatches
                    .AsNoTracking()
                    .FirstOrDefaultAsync(ssm => ssm.SavedSearchId == request.SavedSearchId && ssm.ListingId == existingListing.Id, cancellationToken);

                if (existingMatch == null)
                {
                    var newMatch = new SavedSearchMatch
                    {
                        Id = Guid.NewGuid(),
                        SavedSearchId = request.SavedSearchId,
                        ListingId = existingListing.Id,
                        MatchedAtUtc = scannedAtUtc
                    };
                    _context.SavedSearchMatches.Add(newMatch);
                    itemResult.WasNewMatch = true;
                    result.NewMatchCount++;
                }
                else
                {
                    // Actualizează MatchedAtUtc
                    var matchToUpdate = await _context.SavedSearchMatches
                        .AsTracking()
                        .FirstOrDefaultAsync(ssm => ssm.Id == existingMatch.Id, cancellationToken);
                    if (matchToUpdate != null)
                    {
                        matchToUpdate.MatchedAtUtc = scannedAtUtc;
                    }
                }

                itemResult.WasMatched = true;
                result.MatchedCount++;

                itemResult.PriceChanged = priceChanged;
                itemResult.PriceDropped = priceDropped;
                itemResult.OldPrice = oldPrice;
                itemResult.NewPrice = item.Price;

                result.Items.Add(itemResult);
            }
            catch (Exception ex)
            {
                itemResult.SkipReason = $"Error: {ex.Message}";
                result.SkippedCount++;
                result.Items.Add(itemResult);
            }
        }

        // J. Salvează modificări la final
        await _context.SaveChangesAsync(cancellationToken);

        // I. Actualizează SavedSearch.LastMatchedAtUtc dacă a fost primit cel puțin un listing valid
        if (hasValidListings && result.MatchedCount > 0)
        {
            var ssToUpdate = await _context.SavedSearches
                .AsTracking()
                .FirstOrDefaultAsync(ss => ss.Id == request.SavedSearchId, cancellationToken);
            if (ssToUpdate != null)
            {
                ssToUpdate.LastMatchedAtUtc = scannedAtUtc;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        return result;
    }

    private static string ComputeFallbackHash(string title, int? price, string url)
    {
        var normalizedTitle = title.Trim().ToLowerInvariant();
        var priceStr = price?.ToString() ?? "";
        var normalizedUrl = NormalizeUrlForHash(url);

        var combined = $"{normalizedTitle}|{priceStr}|{normalizedUrl}";

        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combined));
        var hash = BitConverter.ToString(hashedBytes).Replace("-", "").ToLowerInvariant();

        return hash;
    }

    private static string NormalizeUrlForHash(string url)
    {
        var normalized = url.Trim().ToLowerInvariant();

        // Elimină fragmentul după #
        var hashIndex = normalized.IndexOf('#');
        if (hashIndex >= 0)
        {
            normalized = normalized.Substring(0, hashIndex);
        }

        // Elimină trailing slash
        normalized = normalized.TrimEnd('/');

        return normalized;
    }
}
