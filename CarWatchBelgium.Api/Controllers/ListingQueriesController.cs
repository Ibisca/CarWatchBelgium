using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CarWatchBelgium.Application.Listings.Dto;
using CarWatchBelgium.Application.Listings.Services;

namespace CarWatchBelgium.Api.Controllers;

[ApiController]
[Route("api/listing-queries")]
[Authorize]
public class ListingQueriesController : ControllerBase
{
    private readonly IListingQueryService _listingQueryService;
    private readonly ILogger<ListingQueriesController> _logger;

    public ListingQueriesController(IListingQueryService listingQueryService, ILogger<ListingQueriesController> logger)
    {
        _listingQueryService = listingQueryService;
        _logger = logger;
    }

    /// <summary>
    /// Get listings matched by a saved search.
    /// </summary>
    [HttpGet("saved-search/{savedSearchId}")]
    public async Task<ActionResult<List<MatchedListingDto>>> GetBySavedSearch(
        Guid savedSearchId,
        [FromQuery] bool includeUnavailable = false,
        [FromQuery] bool includeIgnored = false,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = User.GetUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized("User ID not found in token.");
            }

            var result = await _listingQueryService.GetBySavedSearchAsync(
                savedSearchId, userId, includeUnavailable, includeIgnored, cancellationToken);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "SavedSearch not found");
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving listings by saved search");
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = "An error occurred." });
        }
    }

    /// <summary>
    /// Get user's favorite listings.
    /// </summary>
    [HttpGet("favorites")]
    public async Task<ActionResult<List<ListingDetailsDto>>> GetFavorites(
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = User.GetUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized("User ID not found in token.");
            }

            var result = await _listingQueryService.GetFavoritesAsync(userId, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving favorites");
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = "An error occurred." });
        }
    }

    /// <summary>
    /// Get unseen listings for a user.
    /// </summary>
    [HttpGet("unseen")]
    public async Task<ActionResult<List<MatchedListingDto>>> GetUnseen(
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = User.GetUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized("User ID not found in token.");
            }

            var result = await _listingQueryService.GetUnseenAsync(userId, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving unseen listings");
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = "An error occurred." });
        }
    }

    /// <summary>
    /// Get detailed information about a listing.
    /// </summary>
    [HttpGet("{listingId}")]
    public async Task<ActionResult<ListingDetailsDto>> GetDetails(
        Guid listingId,
        [FromQuery] Guid userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (userId == Guid.Empty)
            {
                return BadRequest(new { error = "UserId is required." });
            }

            var result = await _listingQueryService.GetDetailsAsync(listingId, userId, cancellationToken);
            
            if (result == null)
            {
                return NotFound(new { error = $"Listing with ID {listingId} not found." });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving listing details");
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = "An error occurred." });
        }
    }
}
