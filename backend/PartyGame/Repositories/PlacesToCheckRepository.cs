﻿using AutoMapper;
using MongoDB.Bson;
using MongoDB.Driver;
using PartyGame.Entities;

namespace PartyGame.Repositories
{
    public interface IPlacesToCheckRepository
    {
        Task<List<PlaceToCheck>> GetAllPlaces();
        void AddNewPlace(PlaceToCheck newPlace);
    }

    public class PlacesToCheckRepository : IPlacesToCheckRepository
    {
        private readonly GameDbContext _gameDbContext;

        public PlacesToCheckRepository(GameDbContext gameDbContext, IMapper mapper)
        {
            _gameDbContext = gameDbContext;
        }

        public async Task<List<PlaceToCheck>> GetAllPlaces()
        {
            return await _gameDbContext.PlacesToCheck.Find(FilterDefinition<PlaceToCheck>.Empty).ToListAsync();
        }

        public async void AddNewPlace(PlaceToCheck newPlace)
        {
            await _gameDbContext.PlacesToCheck.InsertOneAsync(newPlace);
        }

        public async Task<PlaceToCheck> GetPlaceToCheckById(string id)
        {
            return await _gameDbContext.PlacesToCheck
                .Find(p => p.Id == ObjectId.Parse(id))
                .FirstOrDefaultAsync();
        }
    }
}
