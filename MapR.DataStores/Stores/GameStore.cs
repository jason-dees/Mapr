using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MapR.Data.Stores;
using Microsoft.WindowsAzure.Storage.Table;
using MapR.DataStores.Models;
using AutoMapper;
using MapR.DataStores.Extensions;

namespace MapR.DataStores.Stores {

    public class GameStore : CloudTableStore<GameModel>, IStoreGames {

        readonly IMapper _mapper;
        public GameStore(CloudTable gameTable, IMapper mapper) 
            : base(gameTable) {
            _mapper = mapper;
        }

        public async Task<Data.Models.GameModel> GetGame(string owner, string gameId) {
            var game = await GetByRowKey(gameId);
            if(game.Owner != owner) { return null; }
            return game;
        }

        public async Task<Data.Models.GameModel> GetGame(string gameId) {
            return await GetByRowKey(gameId);
        }

        public async Task<IList<Data.Models.GameModel>> GetGames(string owner) {
            return (await GetByPartitionKey(owner)).Select(g => g as Data.Models.GameModel).ToList();
        }

        async Task<bool> IsUniqueId(string gameId) {
            var idQuery = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, gameId);

            var query = new TableQuery<GameModel>()
                .Where(idQuery);

            return !(await _table.ExecuteQuerySegmentedAsync(query, null)).Results.Any();
        }

        public async Task<bool> AddGame(Data.Models.GameModel gameModel) {
			var game = _mapper.Map<GameModel>(gameModel);
            game.GenerateRandomId();

            TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(game);

            await _table.ExecuteAsync(insertOrMergeOperation);
            return true;
        } 
    }
}
