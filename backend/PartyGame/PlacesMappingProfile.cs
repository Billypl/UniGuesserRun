using AutoMapper;
using MongoDB.Bson;
using PartyGame.Entities;
using PartyGame.Models.GameModels;
using PartyGame.Models.PlaceModels;

namespace PartyGame
{
    // Konfiguracja AutoMappera
    public class PlacesMappingProfile : Profile
    {
        public PlacesMappingProfile()
        {
            CreateMap<Place, GuessingPlaceDto>();
            CreateMap<GameSession, SummarizeGameDto>();
            CreateMap<NewPlaceDto, Place>();
            CreateMap<PlaceToCheck, PlaceToCheckDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()));

            CreateMap<PlaceToCheckDto, PlaceToCheck>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => ObjectId.Parse(src.Id))); 
        }


    }
}