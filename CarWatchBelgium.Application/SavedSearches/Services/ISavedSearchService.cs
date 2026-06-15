using CarWatchBelgium.Application.SavedSearches.Dto;

namespace CarWatchBelgium.Application.SavedSearches.Services;

public interface ISavedSearchService
{
    Task<SavedSearchDto> CreateAsync(CreateSavedSearchRequest request, CancellationToken cancellationToken = default);
    Task<List<SavedSearchDto>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<SavedSearchDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<SavedSearchDto?> UpdateAsync(Guid id, UpdateSavedSearchRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeactivateAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
