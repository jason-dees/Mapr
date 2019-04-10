using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using GameModel = MapR.Game.Models.Game;

namespace MapR.Game {
    public interface IStoreGames {
        Task<GameModel> GetGame(string owner, string gameId);
        Task<IList<GameModel>> GetGames(string owner);
        Task<bool> IsUniqueId(string gameId);
    }

    public class GameStore : IStoreGames{
        readonly CloudTable _gameTable;
        public GameStore(CloudTable gameTable) {
            _gameTable = gameTable;
        }

        public Task<GameModel> GetGame(string owner, string gameId) {
            throw new NotImplementedException();
        }

        public Task<IList<GameModel>> GetGames(string owner) {
            throw new NotImplementedException();
        }

        public Task<bool> IsUniqueId(string gameId) {
            throw new NotImplementedException();
        }
    }
}
