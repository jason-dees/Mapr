
using System.Collections.Generic;
using System.Threading.Tasks;
using MapR.Data.Models;

namespace MapR.Data.Stores {
    public interface IStoreMaps {
        Task<bool> AddMap(string owner, string gameId, MapModel map);
        Task<MapModel> GetMap(string owner, string gameId, string mapId);
        Task<MapModel> GetActiveMap(string owner, string gameId);
        Task<IList<MapModel>> GetMaps(string owner, string gameId);
        Task DeleteMap(string owner, string gameId);
		Task<bool> UpdateMap(string owner, string gameId, string mapId, MapModel map);
		Task<bool> ReplaceMapImage(string owner, string gameId, string mapId, MapModel map);
	}
}
