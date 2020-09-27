
using System.Collections.Generic;
using System.Threading.Tasks;
using MapR.Data.Models;

namespace MapR.Data.Stores {
    public interface IStoreMaps {
        Task<string> AddMap(string owner, string gameId, IAmAMapModel map, byte[] imageBytes);
        Task<IAmAMapModel> GetMap(string owner, string gameId, string mapId);
        Task<IAmAMapModel> GetActiveMap(string owner, string gameId);
        Task<IList<IAmAMapModel>> GetMaps(string owner, string gameId);
        Task DeleteMap(string owner, string gameId, string mapId);
		Task<bool> UpdateMap(string owner, string gameId, string mapId, IAmAMapModel map);
        Task<byte[]> GetMapImage(string owner, string gameId, string mapId);
		Task<bool> ReplaceMapImage(string owner, string gameId, string mapId, byte[] imageBytes);

        #region SignalR Hub
        Task<IList<IAmAMapModel>> GetMaps(string gameId);
        Task<IAmAMapModel> GetMap(string gameId, string mapId);
        #endregion
    }
}
