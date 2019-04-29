﻿
using System.Collections.Generic;
using System.Threading.Tasks;
using MapR.Data.Models;

namespace MapR.Data.Stores {
    public interface IStoreMaps {
        Task<bool> AddMap(MapModel map);
        Task<MapModel> GetMap(string mapId);
        Task<IList<MapModel>> GetMaps(string gameId);
        Task DeleteMap(string mapId);
		Task<bool> UpdateMap(MapModel map);
		Task<bool> ReplaceMapImage(MapModel map);
	}
}
