using AutoMapper;
using MediatR;
using UniGuesser.Application.Models.AccountModels;
using UniGuesser.Application.Services;
using UniGuesser.Domain.Entities;
using UniGuesser.Domain.Repositories;

namespace UniGuesser.Application.UseCases.Places.AddNewPlace
{
    public class AddNewPlaceHandle(IPlacesRepository placesRepository, IMapper mapper,
            IAccountRepository accountRepository, IHttpContextAccessorService httpContextAccessorService) : IRequestHandler<AddNewPlaceCommand, Unit>
    {

        public async Task<Unit> Handle(AddNewPlaceCommand newPlace, CancellationToken cancellationToken)
        {
            AccountDetailsFromTokenDto authorData = httpContextAccessorService.GetAuthenticatedUserProfile();

            // Ensure null safety by checking the result of GetByPublicIdAsync
            User? user = await accountRepository.GetByPublicIdAsync(authorData.Guid);
            if (user == null)
            {
                throw new InvalidOperationException($"User with PublicId {authorData.Guid} not found.");
            }

            Place newPlaceToCheck = mapper.Map<Place>(newPlace.newPlaceDto);
            newPlaceToCheck.AuthorId = user.Id;
            newPlaceToCheck.CreatedAt = DateTime.Now;
            newPlaceToCheck.AuthorPlace = user;
            newPlaceToCheck.InQueue = true;
            newPlaceToCheck.DifficultyLevel = newPlace.newPlaceDto.Difficulty;

            await placesRepository.CreateAsync(newPlaceToCheck);

            return Unit.Value; // Ensure the method returns Unit as expected
        }
    }
}
