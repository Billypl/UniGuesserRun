using AutoMapper;
using PartyGame.Entities;
using PartyGame.Models.GameModels;

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
        }


    }
}