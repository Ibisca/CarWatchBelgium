using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CarWatchBelgium.Application.Listings.Dto;
using CarWatchBelgium.Application.Listings.Services;

namespace CarWatchBelgium.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ListingsController : ControllerBase
{
    private readonly IListingIngestionService _listingIngestionService;
    private readonly ILogger<ListingsController> _logger;

    public ListingsController(IListingIngestionService listingIngestionService, ILogger<ListingsController> logger)
    {
        _listingIngestionService = listingIngestionService;
        _logger = logger;
    }

    /// <summary>
    /// Ingest listings from the browser extension.
    /// </summary>
    [HttpPost("ingest")]
    [Authorize]
    public async Task<ActionResult<IngestListingsResult>> IngestListings(
        [FromBody] IngestListingsRequest? request,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (userId == Guid.Empty)
        {
            return Unauthorized("User ID not found in token.");
        }

        try
        {
            if (request == null)
            {
                return BadRequest(new { error = "Request body is required." });
            }

            if (request.SavedSearchId == Guid.Empty)
            {
                return BadRequest(new { error = "SavedSearchId is required and cannot be empty." });
            }

            var result = await _listingIngestionService.IngestAsync(request, cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid SavedSearchId in ingest request");
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during listing ingestion");
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = "An error occurred during listing ingestion." });
        }
    }
}
