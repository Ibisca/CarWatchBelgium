using CarWatchBelgium.Application.Listings.Dto;

namespace CarWatchBelgium.Application.Listings.Services;

public interface IListingStateService
{
    Task<bool> UpdateStateAsync(Guid listingId, UpdateListingStateRequest request, CancellationToken cancellationToken = default);
    Task<bool> MarkSeenAsync(Guid listingId, Guid userId, CancellationToken cancellationToken = default);
    Task<bool> ToggleFavoriteAsync(Guid listingId, Guid userId, CancellationToken cancellationToken = default);
    Task<bool> IgnoreAsync(Guid listingId, Guid userId, CancellationToken cancellationToken = default);
    Task<int> MarkManySeenAsync(MarkListingsSeenRequest request, CancellationToken cancellationToken = default);
}
