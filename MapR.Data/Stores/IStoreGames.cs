using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MapR.Data.Models;

namespace MapR.Data.Stores {
    public interface IStoreGames {
        Task<IAmAGameModel> GetGame(string owner, string gameId);
        Task<IAmAGameModel> GetGame(string gameId);
        Task<IList<IAmAGameModel>> GetGames(string owner);
        Task<IAmAGameModel> AddGame(IAmAGameModel game);
        Task UpdateGame(string owner, string gameId, IAmAGameModel game);
    }
}
