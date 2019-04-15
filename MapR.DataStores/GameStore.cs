using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MapR.Data.Stores;
using MapR.Data.Extensions;
using Microsoft.WindowsAzure.Storage.Table;
using MapR.Data.Models;

namespace MapR.DataStores {

    public class GameStore : IStoreGames{
        readonly CloudTable _gameTable;
        public GameStore(CloudTable gameTable) {
            _gameTable = gameTable;
        }

        public async Task<GameModel> GetGame(string owner, string gameId) {
            var ownerQuery = TableQuery.GenerateFilterCondition("Owner", QueryComparisons.Equal, owner);
            var idQuery = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, gameId);

            var query = new TableQuery<GameModel>()
                .Where(TableQuery.CombineFilters(ownerQuery,
                    TableOperators.And,
                    idQuery));

            return (await _gameTable.ExecuteQuerySegmentedAsync(query, null)).Results.FirstOrDefault();
        }

        public async Task<GameModel> GetGame(string gameId) {
            var idQuery = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, gameId);

            var query = new TableQuery<GameModel>()
                .Where(idQuery);

            return (await _gameTable.ExecuteQuerySegmentedAsync(query, null)).Results.FirstOrDefault();
        }

        public async Task<IList<GameModel>> GetGames(string owner) {
            var ownerQuery = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, owner);

            var query = new TableQuery<GameModel>()
                .Where(ownerQuery);

            return (await _gameTable.ExecuteQuerySegmentedAsync(query, null)).Results;
        }

        async Task<bool> IsUniqueId(string gameId) {
            var idQuery = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, gameId);

            var query = new TableQuery<GameModel>()
                .Where(idQuery);

            return !(await _gameTable.ExecuteQuerySegmentedAsync(query, null)).Results.Any();
        }

        public async Task<bool> AddGame(GameModel game) {

            while(!await IsUniqueId(game.Id)) { //Let's hope and pray it never gets stuck
                game.GenerateRandomId();
            }

            TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(game);

            await _gameTable.ExecuteAsync(insertOrMergeOperation);
            return true;
        } 
    }
}
