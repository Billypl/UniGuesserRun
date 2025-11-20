using UniGuesser.Domain.ValueObjects;

namespace UniGuesser.Application.Models.GeolocationGameModels;

internal class GetDistanceFromPlaceDto
{
    public required string Id;
    public required Coordinates ActualPosition { get; set; }
}