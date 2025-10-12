using UniGuesser.Application.Models.GameModels;
using UniGuesser.Application.UseCases.Games.StartNewGame;

namespace UniGuesser.Application.Services.GameStartStrategies
{
    public interface IStartGameStrategy
    {
        Task<StartedGameData> StartGame(StartNewGameCommand startGameCommand);
    }
}
