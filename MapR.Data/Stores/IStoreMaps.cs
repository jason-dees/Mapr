
using System.Collections.Generic;
using System.Threading.Tasks;
using MapR.Data.Models;

namespace MapR.Data.Stores {
    public interface IStoreMaps {
        Task<string> AddMap(string owner, string gameId, MapModel map, byte[] imageBytes);
        Task<MapModel> GetMap(string owner, string gameId, string mapId);
        Task<MapModel> GetActiveMap(string owner, string gameId);
        Task<IList<MapModel>> GetMaps(string owner, string gameId);
        Task DeleteMap(string owner, string gameId, string mapId);
		Task<bool> UpdateMap(string owner, string gameId, string mapId, MapModel map);
        Task<byte[]> GetMapImage(string owner, string gameId, string mapId);
		Task<bool> ReplaceMapImage(string owner, string gameId, string mapId, byte[] imageBytes);

        #region SignalR Hub
        Task<IList<MapModel>> GetMaps(string gameId);
        Task<MapModel> GetMap(string mapId);
        #endregion
    }
}
