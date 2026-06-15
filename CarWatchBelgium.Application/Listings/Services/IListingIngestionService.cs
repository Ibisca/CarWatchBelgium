using CarWatchBelgium.Application.Listings.Dto;

namespace CarWatchBelgium.Application.Listings.Services;

public interface IListingIngestionService
{
    Task<IngestListingsResult> IngestAsync(IngestListingsRequest request, CancellationToken cancellationToken = default);
}
