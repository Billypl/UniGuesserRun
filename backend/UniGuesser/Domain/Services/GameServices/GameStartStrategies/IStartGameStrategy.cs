using UniGuesser.Application.Models.GameModels;
using UniGuesser.Application.UseCases.Game.StartNewGame;

namespace UniGuesser.Domain.Services.GameServices.GameStartStrategies
{
    public interface IStartGameStrategy
    {
        Task<StartedGameData> StartGame(StartNewGameCommand startData);
    }
}
