using MapR.Data.Models;
using MapR.Data.Stores;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using MapR.CosmosStores.Models;
using AutoMapper;
using System.Linq;

namespace MapR.CosmosStores.Stores {
    public class GameStore : IStoreGames {

        readonly Container _container;
        readonly IMapper _mapper;

        public GameStore(Container container, IMapper mapper) {
            _container = container;
            _mapper = mapper;
        }

        public async Task<bool> AddGame(GameModel game) {
            var newGame = _mapper.Map<Game>(game);
            newGame.Id = GenerateRandomId();
            ItemResponse<Game> response = await _container.CreateItemAsync<Game>(newGame);

            return response.StatusCode == System.Net.HttpStatusCode.Created;
        }

        public async Task<GameModel> GetGame(string owner, string gameId) {
            try {
                ItemResponse<Game> response = await _container.ReadItemAsync<Game>(id: gameId, new PartitionKey(owner));
                return response.Resource;
            }
            catch(CosmosException ex) {
                return null;
            }
        }

        public Task<GameModel> GetGame(string gameId) {
            throw new NotImplementedException();
        }

        public async Task<IList<GameModel>> GetGames(string owner) {
            var sql = $"SELECT * from games as g where g.Owner = \"{owner}\"";
            var queryDefinition = new QueryDefinition(sql);
            var games = new List<Game>();
            var feedIterator = _container.GetItemQueryIterator<Game>(queryDefinition);

            while(feedIterator.HasMoreResults) {
                var gameResponse = await feedIterator.ReadNextAsync();
                games.AddRange(gameResponse);
            }

            return games.ToList<GameModel>();
        }


        public static string GenerateRandomId() {
            return RandomString(6);
        }

        private static readonly Random random = new Random();
        private static string RandomString(int length) {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public async Task UpdateGame(string owner, string gameId, GameModel game) {
            var editedGame = _mapper.Map<Game>(game);
            try {
                await _container.UpsertItemAsync<Game>(editedGame);
            }
            catch (CosmosException ex) {
                
            }
        }
    }
}
