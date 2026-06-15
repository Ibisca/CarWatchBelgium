using System.Security.Claims;

namespace CarWatchBelgium.Api;

public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Extracts the user ID from JWT claims (NameIdentifier or 'sub' claim)
    /// </summary>
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier) ?? user.FindFirst("sub");
        
        if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return userId;
        }

        return Guid.Empty;
    }
}
