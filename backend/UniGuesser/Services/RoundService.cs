using PartyGame.Entities;
using PartyGame.Repositories;

namespace PartyGame.Services
{
    public interface IRoundService
    {
        Task<List<Round>> SaveRounds(List<Round> rounds);
    }

    public class RoundService:IRoundService
    {
        private readonly IRoundRepository _roundsRepository;
        public RoundService(IRoundRepository roundRepository) 
        {
            _roundsRepository = roundRepository;
        }

        public async Task<List<Round>> SaveRounds(List<Round> rounds)
        {
            await _roundsRepository.CreateManyAsync(rounds);
            return rounds;
        }
    }
}
