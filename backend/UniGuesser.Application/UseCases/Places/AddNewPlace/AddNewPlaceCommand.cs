using MediatR;
using Microsoft.AspNetCore.Http;
using UniGuesser.Application.Models.PlaceModels;

namespace UniGuesser.Application.UseCases.Places.AddNewPlace;

public record AddNewPlaceCommand(NewPlaceDto NewPlaceDto, bool InQueue, IFormFile? FormFile) : IRequest<Unit>
{
}