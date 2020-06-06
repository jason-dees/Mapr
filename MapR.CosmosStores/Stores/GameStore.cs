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

        readonly IStoreContainers _internalStore;
        readonly IMapper _mapper;

        public GameStore(IStoreContainers internalStore, IMapper mapper) {
            _internalStore = internalStore;
            _mapper = mapper;
        }

        public async Task<bool> AddGame(GameModel game) {
            var newGame = _mapper.Map<Game>(game);

            return await _internalStore.AddGame(newGame) != null;
        }

        public async Task<GameModel> GetGame(string owner, string gameId) {
            return _mapper.Map<GameModel>(await _internalStore.GetGame(owner, gameId));
        }

        public Task<GameModel> GetGame(string gameId) {
            throw new NotImplementedException();
        }

        public async Task<IList<GameModel>> GetGames(string owner) {
            var games = await _internalStore.GetGames(owner);
            return games.Select(_mapper.Map<GameModel>).ToList();
        }

        public async Task UpdateGame(string owner, string gameId, GameModel game) {
            var editedGame = _mapper.Map<Game>(game);
            await _internalStore.UpdateGame(owner, gameId, editedGame);
        }
    }
}
