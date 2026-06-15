using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CarWatchBelgium.Application.Listings.Dto;
using CarWatchBelgium.Application.Listings.Services;

namespace CarWatchBelgium.Api.Controllers;

[ApiController]
[Route("api/listing-states")]
[Authorize]
public class ListingStatesController : ControllerBase
{
    private readonly IListingStateService _listingStateService;
    private readonly ILogger<ListingStatesController> _logger;

    public ListingStatesController(IListingStateService listingStateService, ILogger<ListingStatesController> logger)
    {
        _listingStateService = listingStateService;
        _logger = logger;
    }

    /// <summary>
    /// Update listing state for the current user.
    /// </summary>
    [HttpPatch("{listingId}")]
    public async Task<IActionResult> UpdateState(
        Guid listingId,
        [FromBody] UpdateCurrentUserListingStateRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = User.GetUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized("User ID not found in token.");
            }

            // Convert to full UpdateListingStateRequest
            var fullRequest = new UpdateListingStateRequest
            {
                UserId = userId,
                IsSeen = request.IsSeen,
                IsFavorite = request.IsFavorite,
                IsIgnored = request.IsIgnored
            };

            var result = await _listingStateService.UpdateStateAsync(listingId, fullRequest, cancellationToken);
            
            if (!result)
            {
                return NotFound(new { error = $"Listing with ID {listingId} not found." });
            }

            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "User not found");
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating listing state");
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = "An error occurred." });
        }
    }

    /// <summary>
    /// Mark a listing as seen.
    /// </summary>
    [HttpPatch("{listingId}/seen")]
    public async Task<IActionResult> MarkSeen(
        Guid listingId,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = User.GetUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized("User ID not found in token.");
            }

            var result = await _listingStateService.MarkSeenAsync(listingId, userId, cancellationToken);
            
            if (!result)
            {
                return NotFound(new { error = $"Listing with ID {listingId} not found." });
            }

            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "User not found");
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking listing as seen");
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = "An error occurred." });
        }
    }

    /// <summary>
    /// Toggle favorite status for a listing.
    /// </summary>
    [HttpPatch("{listingId}/favorite/toggle")]
    public async Task<IActionResult> ToggleFavorite(
        Guid listingId,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = User.GetUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized("User ID not found in token.");
            }

            var result = await _listingStateService.ToggleFavoriteAsync(listingId, userId, cancellationToken);
            
            if (!result)
            {
                return NotFound(new { error = $"Listing with ID {listingId} not found." });
            }

            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "User not found");
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling favorite");
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = "An error occurred." });
        }
    }

    /// <summary>
    /// Mark a listing as ignored.
    /// </summary>
    [HttpPatch("{listingId}/ignore")]
    public async Task<IActionResult> Ignore(
        Guid listingId,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = User.GetUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized("User ID not found in token.");
            }

            var result = await _listingStateService.IgnoreAsync(listingId, userId, cancellationToken);
            
            if (!result)
            {
                return NotFound(new { error = $"Listing with ID {listingId} not found." });
            }

            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "User not found");
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ignoring listing");
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = "An error occurred." });
        }
    }

    /// <summary>
    /// Mark multiple listings as seen.
    /// </summary>
    [HttpPost("mark-seen")]
    public async Task<ActionResult<MarkManySeenResponse>> MarkManySeen(
        [FromBody] MarkCurrentUserListingsSeenRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = User.GetUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized("User ID not found in token.");
            }

            // Convert to full request
            var fullRequest = new MarkListingsSeenRequest
            {
                UserId = userId,
                ListingIds = request.ListingIds
            };

            var markedCount = await _listingStateService.MarkManySeenAsync(fullRequest, cancellationToken);
            
            return Ok(new MarkManySeenResponse { MarkedCount = markedCount });
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "User not found");
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking many listings as seen");
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = "An error occurred." });
        }
    }
}

public class MarkManySeenResponse
{
    public int MarkedCount { get; set; }
}
