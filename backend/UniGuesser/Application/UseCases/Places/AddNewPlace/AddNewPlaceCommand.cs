using MediatR;
using UniGuesser.Application.Models.AccountModels;
using UniGuesser.Application.Models.PlaceModels;

namespace UniGuesser.Application.UseCases.Places.AddNewPlace
{
    public record AddNewPlaceCommand(NewPlaceDto newPlaceDto,bool inQueue) : IRequest<Unit>
    {
       
    }
}
