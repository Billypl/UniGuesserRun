using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniGuesser.Application.Models.GameModels;
using UniGuesser.Application.Services;
using UniGuesser.Application.UseCases.GameSessions.GetGameDetails;
using UniGuesser.Domain.Entities;
using UniGuesser.Domain.Exceptions;
using UniGuesser.Domain.Services;
using UniGuesser.Domain.ValueObjects;
using UniGuesser.Domain.ValueObjects.Enumerations;
using UniGuesser.Infrastructure;
using IGameSessionRepository = UniGuesser.Infrastructure.IGameSessionRepository;

namespace UniGuesser.Application.UseCases.GeolocationGames.GetDistanceFromPlace
{
    public class GetDistanceFromPlaceHandler(
        IGameSessionRepository gameSessionRepository, 
        IHttpContextAccessorService httpContextAccessorService) 
        :IRequestHandler<GetDistanceFromPlaceCommand, double>
    {
        public async Task<double> Handle(GetDistanceFromPlaceCommand request, CancellationToken cancellationToken)
        {
            GameSession? session = await gameSessionRepository.GetAsync(request.gameId);

            if(session == null)
            {
                throw new GameSessionExceptions.GameNotFoundException(request.gameId);
            }

            if (session.GameMode != GameMode.Geolocation)
            {
                throw new GameExceptions.WrongGameModeException(session.GameMode.ToString());
            }

            var actualRound = session.Rounds[session.ActualRoundNumber];

            var distance = DistanceCalculator.CalculateDistanceBetweenCords(new Coordinates
                { Latitude = actualRound.PlaceToGuess.Latitude, Longitude = actualRound.PlaceToGuess.Longitude }, request.playerPosition);

            return distance;
        }
    }
}
