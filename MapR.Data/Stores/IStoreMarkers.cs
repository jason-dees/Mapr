using System.Threading.Tasks;
using MapR.Data.Models;
using System.Collections.Generic;

namespace MapR.Data.Stores {
    public interface IStoreMarkers {
        Task AddMarker(MarkerModel marker);
        Task UpdateMarker(MarkerModel marker);
        Task DeleteMarker(MarkerModel marker);
        Task<MarkerModel> GetMarker(string gameId, string mapId, string markerId);
        Task<IList<MarkerModel>> GetMarkers(string gameId, string mapId);
    }
}
