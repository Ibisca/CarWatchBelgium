using Microsoft.EntityFrameworkCore;
using CarWatchBelgium.Application.Listings.Dto;
using CarWatchBelgium.Application.Listings.Services;
using CarWatchBelgium.Domain.Entities;
using CarWatchBelgium.Infrastructure.Persistence;

namespace CarWatchBelgium.Infrastructure.Listings;

public class ListingStateService : IListingStateService
{
    private readonly AppDbContext _context;

    public ListingStateService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> UpdateStateAsync(Guid listingId, UpdateListingStateRequest request, CancellationToken cancellationToken = default)
    {
        // Verifică user-ul
        var userExists = await _context.Users
            .AsNoTracking()
            .AnyAsync(u => u.Id == request.UserId, cancellationToken);

        if (!userExists)
        {
            throw new KeyNotFoundException($"User with ID {request.UserId} does not exist.");
        }

        // Verifică listing-ul
        var listing = await _context.Listings
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == listingId, cancellationToken);

        if (listing == null)
            return false;

        // Caută UserListingState
        var userState = await _context.UserListingStates
            .AsTracking()
            .FirstOrDefaultAsync(us => us.UserId == request.UserId && us.ListingId == listingId, cancellationToken);

        // Creează dacă nu există
        if (userState == null)
        {
            userState = new UserListingState
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                ListingId = listingId,
                IsSeen = false,
                IsFavorite = false,
                IsIgnored = false,
                CreatedAtUtc = DateTime.UtcNow
            };
            _context.UserListingStates.Add(userState);
        }

        // Actualizează câmpurile
        if (request.IsSeen.HasValue)
        {
            userState.IsSeen = request.IsSeen.Value;
            if (request.IsSeen.Value && userState.SeenAtUtc == null)
            {
                userState.SeenAtUtc = DateTime.UtcNow;
            }
        }

        if (request.IsFavorite.HasValue)
        {
            userState.IsFavorite = request.IsFavorite.Value;
            if (request.IsFavorite.Value)
            {
                userState.FavoritedAtUtc = DateTime.UtcNow;
            }
            else
            {
                userState.FavoritedAtUtc = null;
            }
        }

        if (request.IsIgnored.HasValue)
        {
            userState.IsIgnored = request.IsIgnored.Value;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> MarkSeenAsync(Guid listingId, Guid userId, CancellationToken cancellationToken = default)
    {
        // Verifică user-ul
        var userExists = await _context.Users
            .AsNoTracking()
            .AnyAsync(u => u.Id == userId, cancellationToken);

        if (!userExists)
        {
            throw new KeyNotFoundException($"User with ID {userId} does not exist.");
        }

        // Verifică listing-ul
        var listing = await _context.Listings
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == listingId, cancellationToken);

        if (listing == null)
            return false;

        // Caută UserListingState
        var userState = await _context.UserListingStates
            .AsTracking()
            .FirstOrDefaultAsync(us => us.UserId == userId && us.ListingId == listingId, cancellationToken);

        // Creează dacă nu există
        if (userState == null)
        {
            userState = new UserListingState
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ListingId = listingId,
                IsSeen = true,
                SeenAtUtc = DateTime.UtcNow,
                IsFavorite = false,
                IsIgnored = false,
                CreatedAtUtc = DateTime.UtcNow
            };
            _context.UserListingStates.Add(userState);
        }
        else
        {
            userState.IsSeen = true;
            if (userState.SeenAtUtc == null)
            {
                userState.SeenAtUtc = DateTime.UtcNow;
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> ToggleFavoriteAsync(Guid listingId, Guid userId, CancellationToken cancellationToken = default)
    {
        // Verifică user-ul
        var userExists = await _context.Users
            .AsNoTracking()
            .AnyAsync(u => u.Id == userId, cancellationToken);

        if (!userExists)
        {
            throw new KeyNotFoundException($"User with ID {userId} does not exist.");
        }

        // Verifică listing-ul
        var listing = await _context.Listings
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == listingId, cancellationToken);

        if (listing == null)
            return false;

        // Caută UserListingState
        var userState = await _context.UserListingStates
            .AsTracking()
            .FirstOrDefaultAsync(us => us.UserId == userId && us.ListingId == listingId, cancellationToken);

        // Creează dacă nu există
        if (userState == null)
        {
            userState = new UserListingState
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ListingId = listingId,
                IsSeen = false,
                IsFavorite = true,
                FavoritedAtUtc = DateTime.UtcNow,
                IsIgnored = false,
                CreatedAtUtc = DateTime.UtcNow
            };
            _context.UserListingStates.Add(userState);
        }
        else
        {
            userState.IsFavorite = !userState.IsFavorite;
            if (userState.IsFavorite)
            {
                userState.FavoritedAtUtc = DateTime.UtcNow;
            }
            else
            {
                userState.FavoritedAtUtc = null;
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> IgnoreAsync(Guid listingId, Guid userId, CancellationToken cancellationToken = default)
    {
        // Verifică user-ul
        var userExists = await _context.Users
            .AsNoTracking()
            .AnyAsync(u => u.Id == userId, cancellationToken);

        if (!userExists)
        {
            throw new KeyNotFoundException($"User with ID {userId} does not exist.");
        }

        // Verifică listing-ul
        var listing = await _context.Listings
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == listingId, cancellationToken);

        if (listing == null)
            return false;

        // Caută UserListingState
        var userState = await _context.UserListingStates
            .AsTracking()
            .FirstOrDefaultAsync(us => us.UserId == userId && us.ListingId == listingId, cancellationToken);

        // Creează dacă nu există
        if (userState == null)
        {
            userState = new UserListingState
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ListingId = listingId,
                IsSeen = false,
                IsFavorite = false,
                IsIgnored = true,
                CreatedAtUtc = DateTime.UtcNow
            };
            _context.UserListingStates.Add(userState);
        }
        else
        {
            userState.IsIgnored = true;
            userState.IsFavorite = false;
            userState.FavoritedAtUtc = null;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<int> MarkManySeenAsync(MarkListingsSeenRequest request, CancellationToken cancellationToken = default)
    {
        // Verifică user-ul
        var userExists = await _context.Users
            .AsNoTracking()
            .AnyAsync(u => u.Id == request.UserId, cancellationToken);

        if (!userExists)
        {
            throw new KeyNotFoundException($"User with ID {request.UserId} does not exist.");
        }

        if (request.ListingIds == null || request.ListingIds.Count == 0)
            return 0;

        var listingIds = request.ListingIds.Distinct().ToList();

        // Găsește listing-urile existente
        var existingListings = await _context.Listings
            .AsNoTracking()
            .Where(l => listingIds.Contains(l.Id))
            .Select(l => l.Id)
            .ToListAsync(cancellationToken);

        if (existingListings.Count == 0)
            return 0;

        // Găsește UserListingStates existente pentru acest user
        var existingStates = await _context.UserListingStates
            .AsTracking()
            .Where(us => us.UserId == request.UserId && existingListings.Contains(us.ListingId))
            .ToListAsync(cancellationToken);

        var statesByListingId = existingStates.ToDictionary(s => s.ListingId);
        int markedCount = 0;

        // Actualizează existente
        foreach (var state in existingStates)
        {
            if (!state.IsSeen)
            {
                state.IsSeen = true;
                if (state.SeenAtUtc == null)
                {
                    state.SeenAtUtc = DateTime.UtcNow;
                }
                markedCount++;
            }
        }

        // Creează noi pentru listing-uri fără UserListingState
        foreach (var listingId in existingListings)
        {
            if (!statesByListingId.ContainsKey(listingId))
            {
                var newState = new UserListingState
                {
                    Id = Guid.NewGuid(),
                    UserId = request.UserId,
                    ListingId = listingId,
                    IsSeen = true,
                    SeenAtUtc = DateTime.UtcNow,
                    IsFavorite = false,
                    IsIgnored = false,
                    CreatedAtUtc = DateTime.UtcNow
                };
                _context.UserListingStates.Add(newState);
                markedCount++;
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        return markedCount;
    }
}
