using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CarWatchBelgium.Application.SavedSearches.Dto;
using CarWatchBelgium.Application.SavedSearches.Services;

namespace CarWatchBelgium.Api.Controllers;

[ApiController]
[Route("api/saved-searches")]
[Authorize]
public class SavedSearchesController : ControllerBase
{
    private readonly ISavedSearchService _savedSearchService;
    private readonly ILogger<SavedSearchesController> _logger;

    public SavedSearchesController(ISavedSearchService savedSearchService, ILogger<SavedSearchesController> logger)
    {
        _savedSearchService = savedSearchService;
        _logger = logger;
    }

    /// <summary>
    /// Create a new saved search for the current authenticated user.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<SavedSearchDto>> CreateSavedSearch(
        [FromBody] CreateSavedSearchForCurrentUserRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (userId == Guid.Empty)
        {
            return Unauthorized("User ID not found in token.");
        }

        try
        {
            // Convert to CreateSavedSearchRequest with the authenticated user ID
            var createRequest = new CreateSavedSearchRequest
            {
                UserId = userId,
                Name = request.Name,
                Make = request.Make,
                Model = request.Model,
                CountryCode = request.CountryCode,
                PriceMin = request.PriceMin,
                PriceMax = request.PriceMax,
                YearMin = request.YearMin,
                YearMax = request.YearMax,
                MaxMileageKm = request.MaxMileageKm,
                FuelType = request.FuelType,
                Transmission = request.Transmission,
                MinPowerHp = request.MinPowerHp,
                SellerType = request.SellerType,
                City = request.City,
                RadiusKm = request.RadiusKm,
                RequiredKeywords = request.RequiredKeywords,
                ExcludedKeywords = request.ExcludedKeywords
            };

            var result = await _savedSearchService.CreateAsync(createRequest, cancellationToken);
            return CreatedAtAction(nameof(GetSavedSearchById), new { id = result.Id }, result);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "User not found in create saved search");
            return NotFound(new { error = ex.Message });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validation error in create saved search");
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating saved search");
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = "An error occurred while creating the saved search." });
        }
    }

    /// <summary>
    /// Get all saved searches for the current authenticated user.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<SavedSearchDto>>> GetSavedSearches(
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (userId == Guid.Empty)
        {
            return Unauthorized("User ID not found in token.");
        }

        try
        {
            var result = await _savedSearchService.GetByUserAsync(userId, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving saved searches for user {UserId}", userId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = "An error occurred while retrieving saved searches." });
        }
    }

    /// <summary>
    /// Get a saved search by ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<SavedSearchDto>> GetSavedSearchById(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _savedSearchService.GetByIdAsync(id, cancellationToken);
            if (result == null)
            {
                return NotFound(new { error = $"Saved search with ID {id} not found." });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving saved search {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = "An error occurred while retrieving the saved search." });
        }
    }

    /// <summary>
    /// Update a saved search.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<SavedSearchDto>> UpdateSavedSearch(
        Guid id,
        [FromBody] UpdateSavedSearchRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _savedSearchService.UpdateAsync(id, request, cancellationToken);
            if (result == null)
            {
                return NotFound(new { error = $"Saved search with ID {id} not found." });
            }

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validation error in update saved search");
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating saved search {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = "An error occurred while updating the saved search." });
        }
    }

    /// <summary>
    /// Deactivate a saved search.
    /// </summary>
    [HttpPatch("{id}/deactivate")]
    public async Task<IActionResult> DeactivateSavedSearch(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _savedSearchService.DeactivateAsync(id, cancellationToken);
            if (!result)
            {
                return NotFound(new { error = $"Saved search with ID {id} not found." });
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deactivating saved search {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = "An error occurred while deactivating the saved search." });
        }
    }

    /// <summary>
    /// Delete a saved search.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSavedSearch(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _savedSearchService.DeleteAsync(id, cancellationToken);
            if (!result)
            {
                return NotFound(new { error = $"Saved search with ID {id} not found." });
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting saved search {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = "An error occurred while deleting the saved search." });
        }
    }
}
