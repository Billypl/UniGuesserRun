using AutoMapper;
using MongoDB.Bson;
using PartyGame.Entities;
using PartyGame.Models;
using PartyGame.Models.AccountModels;
using PartyGame.Models.GameModels;
using PartyGame.Models.PlaceModels;
using PartyGame.Models.ScoreboardModels;

namespace PartyGame.Extensions
{
    // Konfiguracja AutoMappera
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Place, GuessingPlaceDto>();
            CreateMap<NewPlaceDto, Place>();
            CreateMap<PlaceToCheck, PlaceToCheckDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()));

            CreateMap<PlaceToCheckDto, PlaceToCheck>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => ObjectId.Parse(src.Id)));

            CreateMap<User, AccountDetailsDto>();
            CreateMap<AccountDetailsDto, User>();

            CreateMap<UpdatePlaceDto, Place>();
            CreateMap<Place, ShowPlaceDto>()
                  .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString())); ;

            CreateMap<GameSession, FinishedGame>()
                .ForMember(dest => dest.FinalScore, opt => opt.MapFrom(src => src.GameScore));

            CreateMap<FinishedGame,FinishedGameDto>()
                  .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                  .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId.ToString()))
                  .ForMember(dest => dest.DifficultyLevel, opt => opt.MapFrom(src => src.DifficultyLevel.ToString()));

            CreateMap<FinishedGame, FinishedGameToTable>()
                  .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                  .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId.ToString()))
                  .ForMember(dest => dest.DifficultyLevel, opt => opt.MapFrom(src => src.DifficultyLevel.ToString()));

            CreateMap<ShowPlaceDto, GuessingPlaceDto>();

            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));

            CreateMap<GameSession, GameSessionStateDto>();

        }


    }
}