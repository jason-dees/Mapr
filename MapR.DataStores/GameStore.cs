using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MapR.Data.Stores;
using Microsoft.WindowsAzure.Storage.Table;
using MapR.DataStores.Models;
using MapR.DataStores.Configuration;
using AutoMapper;
using MapR.DataStores.Extensions;

namespace MapR.DataStores {

    public class GameStore : IStoreGames{
        readonly CloudTable _gameTable;
		readonly IMapper _mapper;
		public GameStore(CloudTable gameTable, IAmDataStoreMapper mapper) {
            _gameTable = gameTable;
			_mapper = mapper;
        }

        public async Task<Data.Models.GameModel> GetGame(string owner, string gameId) {
            var ownerQuery = TableQuery.GenerateFilterCondition("Owner", QueryComparisons.Equal, owner);
            var idQuery = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, gameId);

            var query = new TableQuery<GameModel>()
                .Where(TableQuery.CombineFilters(ownerQuery,
                    TableOperators.And,
                    idQuery));

            return (await _gameTable.ExecuteQuerySegmentedAsync(query, null)).Results.FirstOrDefault();
        }

        public async Task<Data.Models.GameModel> GetGame(string gameId) {
            var idQuery = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, gameId);

            var query = new TableQuery<GameModel>()
                .Where(idQuery);

            return (await _gameTable.ExecuteQuerySegmentedAsync(query, null)).Results.FirstOrDefault();
        }

        public async Task<IList<Data.Models.GameModel>> GetGames(string owner) {
            var ownerQuery = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, owner);

            var query = new TableQuery<GameModel>()
                .Where(ownerQuery);

            return (IList<MapR.Data.Models.GameModel>)(await _gameTable.ExecuteQuerySegmentedAsync(query, null)).Results;
        }

        async Task<bool> IsUniqueId(string gameId) {
            var idQuery = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, gameId);

            var query = new TableQuery<GameModel>()
                .Where(idQuery);

            return !(await _gameTable.ExecuteQuerySegmentedAsync(query, null)).Results.Any();
        }

        public async Task<bool> AddGame(Data.Models.GameModel gameModel) {
			var game = _mapper.Map<GameModel>(gameModel);
            while(!await IsUniqueId(game.Id)) { //Let's hope and pray it never gets stuck
                game.GenerateRandomId();
            }

            TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(game);

            await _gameTable.ExecuteAsync(insertOrMergeOperation);
            return true;
        } 
    }
}
