using AutoMapper;
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

            CreateMap<User, AccountDetailsDto>();
            CreateMap<AccountDetailsDto, User>();

            CreateMap<UpdatePlaceDto, Place>();
            CreateMap<Place, ShowPlaceDto>()
                  .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.PublicId.ToString()))
                  .ForMember(dest => dest.Coordinates, opt => opt.MapFrom(src =>
                  new Coordinates { Latitude = src.Latitude, Longitude = src.Longitude }))
                  .ForMember(dest => dest.AuthorId, opt => opt.MapFrom(src => src.AuthorPlace.PublicId))
                  .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.AuthorPlace.Nickname));

            CreateMap<GameSession, FinishedGameDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.PublicId.ToString()))  // Map PublicId to Id as string
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId.HasValue ? src.UserId.ToString() : null))  // Map nullable UserId to string
            .ForMember(dest => dest.Nickname, opt => opt.MapFrom(src => src.Player != null ? src.Player.Nickname : null))  // Map nullable Player Nickname
            .ForMember(dest => dest.FinalScore, opt => opt.MapFrom(src => src.GameScore))  // Map GameScore to FinalScore
            .ForMember(dest => dest.Rounds, opt => opt.MapFrom(src => src.Rounds))  // Map Rounds
            .ForMember(dest => dest.Difficulty, opt => opt.MapFrom(src => src.Difficulty));  // Map Difficulty

            CreateMap<ShowPlaceDto, GuessingPlaceDto>();

            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));

            CreateMap<GameSession, GameSessionStateDto>();

            CreateMap<Round, GuessingPlaceDto>();

        }
    }
}