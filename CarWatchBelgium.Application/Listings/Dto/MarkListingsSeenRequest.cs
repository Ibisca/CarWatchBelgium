namespace CarWatchBelgium.Application.Listings.Dto;

public class MarkListingsSeenRequest
{
    public Guid UserId { get; set; }
    public List<Guid> ListingIds { get; set; } = new();
}
