namespace CarWatchBelgium.Application.Listings.Dto;

public class MarkCurrentUserListingsSeenRequest
{
    public List<Guid> ListingIds { get; set; } = new();
}
