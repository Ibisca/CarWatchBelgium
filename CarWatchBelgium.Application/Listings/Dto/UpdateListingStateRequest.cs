namespace CarWatchBelgium.Application.Listings.Dto;

public class UpdateListingStateRequest
{
    public Guid UserId { get; set; }
    public bool? IsSeen { get; set; }
    public bool? IsFavorite { get; set; }
    public bool? IsIgnored { get; set; }
}
