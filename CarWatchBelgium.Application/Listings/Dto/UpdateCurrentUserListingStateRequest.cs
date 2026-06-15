namespace CarWatchBelgium.Application.Listings.Dto;

public class UpdateCurrentUserListingStateRequest
{
    public bool? IsSeen { get; set; }
    public bool? IsFavorite { get; set; }
    public bool? IsIgnored { get; set; }
}
