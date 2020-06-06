using MapR.CosmosStores.Models;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MapR.CosmosStores.Stores.Internal {
    public interface IStoreContainers {

        Task<Game> AddGame(Game game);
        Task<Game> GetGame(string owner, string gameId);
        Task<IList<Game>> GetGames(string owner);
        Task UpdateGame(string owner, string gameId, Game game);

    }
    internal class ContainerStore : IStoreContainers {

        readonly Container _container;

        public ContainerStore(Container container) {
            _container = container;
        }

        public async Task<Game> AddGame(Game newGame) {
            newGame.Id = GenerateRandomId();
            ItemResponse<Game> response = await _container.CreateItemAsync<Game>(newGame);
            if (response.StatusCode == System.Net.HttpStatusCode.Created) {
                return await GetGame(newGame.Owner, newGame.Id);
            }
            return null;
        }

        public async Task<Game> GetGame(string owner, string gameId) {
            try {
                ItemResponse<Game> response = await _container.ReadItemAsync<Game>(id: gameId, new PartitionKey(owner));
                return response.Resource;
            }
            catch (CosmosException ex) {
                return null;
            }
        }

        public async Task<IList<Game>> GetGames(string owner) {
            var sql = $"SELECT * from games as g where g.Owner = \"{owner}\"";
            var queryDefinition = new QueryDefinition(sql);
            var games = new List<Game>();
            var feedIterator = _container.GetItemQueryIterator<Game>(queryDefinition);

            while (feedIterator.HasMoreResults) {
                var gameResponse = await feedIterator.ReadNextAsync();
                games.AddRange(gameResponse);
            }
            return games;
        }

        public async Task UpdateGame(string owner, string gameId, Game game) {
            try {
                await _container.UpsertItemAsync<Game>(game);
            }
            catch (CosmosException ex) {

            }
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
    }
}
