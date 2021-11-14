using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MapR.Data.Stores;
using MapR.DataStores.Models;
using AutoMapper;
using MapR.DataStores.Extensions;

namespace MapR.DataStores.Stores {

    public class GameStore : IStoreGames {

        readonly IMapper _mapper;
        readonly IAccessCloudTableData<GameModel> _cloudTableAccess;

        public GameStore(IAccessCloudTableData<GameModel> cloudTableAccess, IMapper mapper)
        {
            _cloudTableAccess = cloudTableAccess;
            _mapper = mapper;
        }

        public async Task<Data.Models.IAmAGameModel> GetGame(string owner, string gameId) {
            var game = await _cloudTableAccess.GetByRowKey(gameId);
            if(game.Owner != owner) { return null; }
            return game;
        }

        public async Task<Data.Models.IAmAGameModel> GetGame(string gameId) {
            return await _cloudTableAccess.GetByRowKey(gameId);
        }

        public async Task<IList<Data.Models.IAmAGameModel>> GetGames(string owner) {
            return (await _cloudTableAccess.GetByPartitionKey(owner)).Select(g => g as Data.Models.IAmAGameModel).ToList();
        }

        async Task<bool> IsUniqueId(string gameId) {
            return await _cloudTableAccess.IsUniqueKey(gameId);
        }

        public async Task<Data.Models.IAmAGameModel> AddGame(Data.Models.IAmAGameModel gameModel) {
		    var game = _mapper.Map<GameModel>(gameModel);
            game.GenerateRandomId();
            await _cloudTableAccess.InsertOrMerge(game);
            return await GetGame(game.Id);
        }

        public Task UpdateGame(string owner, string gameId, Data.Models.IAmAGameModel game) {
            throw new System.NotImplementedException();
        }
    }
}
