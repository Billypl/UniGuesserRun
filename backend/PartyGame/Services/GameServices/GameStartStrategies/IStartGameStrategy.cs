using PartyGame.Models.GameModels;

namespace PartyGame.Services.GameServices.GameStartStrategies
{
    public interface IStartGameStrategy
    {
        Task<string> StartGame(StartDataDto startData);
    }






}
