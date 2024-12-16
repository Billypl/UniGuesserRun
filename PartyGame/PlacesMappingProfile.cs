using AutoMapper;
using PartyGame.Entities;
using PartyGame.Models;

namespace PartyGame
{
    // Konfiguracja AutoMappera
    public class PlacesMappingProfile : Profile
    {
        public PlacesMappingProfile()
        {
            // Mapowanie z GameSession na StartDataDto
            CreateMap<GameSession, StartDataDto>()
                .ForMember(dest => dest.Token, opt => opt.MapFrom(src => src.Token))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Place.ImageUrl))
                .ForMember(dest => dest.GameId, opt => opt.MapFrom(src => src.Id));
        }
    }
}