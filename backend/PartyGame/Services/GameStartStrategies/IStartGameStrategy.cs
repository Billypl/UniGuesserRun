

using PartyGame.Models.GameModels;

namespace PartyGame.Services.StartGame
{
    public interface IStartGameStrategy
    {
        Task<string> StartGame(StartDataDto startData);
    }


   


   
}
