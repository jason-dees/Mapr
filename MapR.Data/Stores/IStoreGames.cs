using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MapR.Data.Models;

namespace MapR.Data.Stores {
    public interface IStoreGames {
        Task<GameModel> GetGame(string owner, string gameId);
        Task<GameModel> GetGame(string gameId);
        Task<IList<GameModel>> GetGames(string owner);
        Task<bool> AddGame(GameModel game);
    }
}
