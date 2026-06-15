using System.Security.Claims;

namespace CarWatchBelgium.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        // Try to get from NameIdentifier first
        var nameIdentifierClaim = user.FindFirst(ClaimTypes.NameIdentifier);
        if (nameIdentifierClaim != null && Guid.TryParse(nameIdentifierClaim.Value, out var userId))
        {
            return userId;
        }

        // Fall back to "sub" claim
        var subClaim = user.FindFirst("sub");
        if (subClaim != null && Guid.TryParse(subClaim.Value, out userId))
        {
            return userId;
        }

        throw new UnauthorizedAccessException("User ID not found in token claims.");
    }
}
