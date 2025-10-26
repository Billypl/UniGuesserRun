using AutoMapper;
using MediatR;
using UniGuesser.Application.Models.AccountModels;
using UniGuesser.Application.Services;
using UniGuesser.Application.Services.SaveFileService;
using UniGuesser.Domain.Entities;
using UniGuesser.Domain.Repositories;
using UniGuesser.Domain.ValueObjects.Enumerations;

namespace UniGuesser.Application.UseCases.Places.AddNewPlace
{
    public class AddNewPlaceHandle(IPlacesRepository placesRepository, IMapper mapper,
            IAccountRepository accountRepository, IHttpContextAccessorService httpContextAccessorService, IFileService fileService) : IRequestHandler<AddNewPlaceCommand, Unit>
    {

        public async Task<Unit> Handle(AddNewPlaceCommand newPlace, CancellationToken cancellationToken)
        {
            AccountDetailsFromTokenDto authorData = httpContextAccessorService.GetAuthenticatedUserProfile();

            // Ensure null safety by checking the result of GetByPublicIdAsync
            User? user = await accountRepository.GetAsync(Guid.Parse(authorData.Guid));
            if (user == null)
            {
                throw new InvalidOperationException($"User with Id {authorData.Guid} not found.");
            }

            Place newPlaceToCheck = mapper.Map<Place>(newPlace.NewPlaceDto);

            newPlaceToCheck.AuthorId = user.Id;
            newPlaceToCheck.CreatedAt = DateTime.Now;
            newPlaceToCheck.AuthorPlace = user;
            newPlaceToCheck.InQueue = newPlace.InQueue;
            newPlaceToCheck.DifficultyLevel =
                (DifficultyLevel)Enum.Parse(typeof(DifficultyLevel), newPlace.NewPlaceDto.Difficulty, true);

            if (newPlace.FormFile is null && newPlace.NewPlaceDto.ImageType == ImageType.File)
            {
                throw new ArgumentNullException(nameof(newPlace.FormFile), "FormFile cannot be null when ImageType is File.");
            }

            if(newPlace.NewPlaceDto.ImageType == ImageType.File)
                newPlaceToCheck.ImageUrl = await fileService.SaveFile(newPlace.FormFile);

            await placesRepository.CreateAsync(newPlaceToCheck);

            return Unit.Value; 
        }
    }
}
