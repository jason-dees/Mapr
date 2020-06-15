using MapR.Data.Models;
using MapR.Data.Stores;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using MapR.CosmosStores.Models;
using AutoMapper;
using System.Linq;
using MapR.CosmosStores.Stores.Internal;

namespace MapR.CosmosStores.Stores {
    public class GameStore : IStoreGames {

        readonly IAmAGameContainerHelper _containerHelper;
        readonly IMapper _mapper;

        public GameStore(IAmAGameContainerHelper containerHelper, IMapper mapper) {
            _containerHelper = containerHelper  ;
            _mapper = mapper;
        }

        public async Task<GameModel> AddGame(GameModel game) {
            var newGame = _mapper.Map<Game>(game);
            return _mapper.Map<GameModel>(await _containerHelper.AddGame(newGame));
        }

        public async Task<GameModel> GetGame(string owner, string gameId) {
            return _mapper.Map<GameModel>(await _containerHelper.GetGame(owner, gameId));
        }

        public Task<GameModel> GetGame(string gameId) {
            throw new NotImplementedException();
        }

        public async Task<IList<GameModel>> GetGames(string owner) {
            var games = await _containerHelper.GetGames(owner);
            return games.Select(_mapper.Map<GameModel>).ToList();
        }

        public async Task UpdateGame(string owner, string gameId, GameModel game) {
            var editedGame = _mapper.Map<Game>(game);
            await _containerHelper.UpdateGame(owner, gameId, editedGame);
        }

        public async Task DeleteGame(string owner, string gameId) {

        }
    }
}
