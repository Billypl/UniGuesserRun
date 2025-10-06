using UniGuesser.Application.Models.GameModels;

namespace UniGuesser.Domain.Services.GameServices.GameStartStrategies
{
    public interface IStartGameStrategy
    {
        Task<StartedGameData> StartGame(StartDataDto startData);
    }






}
