using Models.GameModels;

namespace Services.GameServices.GameStartStrategies
{
    public interface IStartGameStrategy
    {
        Task<StartedGameData> StartGame(StartDataDto startData);
    }






}
