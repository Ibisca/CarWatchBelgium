using CarWatchBelgium.Application.Listings.Dto;

namespace CarWatchBelgium.Application.Listings.Services;

public interface IListingQueryService
{
    Task<List<MatchedListingDto>> GetBySavedSearchAsync(Guid savedSearchId, Guid userId, bool includeUnavailable = false, bool includeIgnored = false, CancellationToken cancellationToken = default);
    Task<List<ListingDetailsDto>> GetFavoritesAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<List<MatchedListingDto>> GetUnseenAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<ListingDetailsDto?> GetDetailsAsync(Guid listingId, Guid userId, CancellationToken cancellationToken = default);
}
