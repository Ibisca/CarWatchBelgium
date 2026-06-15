using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using CarWatchBelgium.Domain.Entities;
using CarWatchBelgium.Infrastructure.Persistence;

namespace CarWatchBelgium.Api.Controllers;

/// <summary>
/// Development controller for user management.
/// TODO: Remove this controller after JWT authentication is implemented.
/// </summary>
[ApiController]
[Route("api/dev/users")]
public class DevUsersController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<DevUsersController> _logger;

    public DevUsersController(AppDbContext context, ILogger<DevUsersController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Create or get a development user (temporary endpoint).
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<DevUserResponse>> CreateOrGetUser(
        [FromBody] CreateDevUserRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return BadRequest(new { error = "Email is required." });
            }

            var email = request.Email.Trim().ToLowerInvariant();
            var displayName = string.IsNullOrWhiteSpace(request.DisplayName) ? null : request.DisplayName.Trim();

            // Check if user exists
            var existingUser = await _context.Users
                .AsTracking()
                .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

            if (existingUser != null)
            {
                _logger.LogInformation("Returning existing user with email {Email}", email);
                return Ok(new DevUserResponse
                {
                    Id = existingUser.Id,
                    Email = existingUser.Email,
                    DisplayName = existingUser.DisplayName
                });
            }

            // Create new user
            var newUser = new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                PasswordHash = "DEV_ONLY",
                DisplayName = displayName,
                CreatedAtUtc = DateTime.UtcNow,
                IsActive = true
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Created new development user with email {Email}", email);

            return CreatedAtAction(nameof(CreateOrGetUser), new DevUserResponse
            {
                Id = newUser.Id,
                Email = newUser.Email,
                DisplayName = newUser.DisplayName
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating or retrieving development user");
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = "An error occurred." });
        }
    }
}

public class CreateDevUserRequest
{
    public string Email { get; set; } = null!;
    public string? DisplayName { get; set; }
}

public class DevUserResponse
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string? DisplayName { get; set; }
}
