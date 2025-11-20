using MediatR;
using UniGuesser.Application.Models.GameModels;

namespace UniGuesser.Application.UseCases.Games.GetActiveGame;

public record GetActiveGameQuery : IRequest<GameSessionStateDto>;