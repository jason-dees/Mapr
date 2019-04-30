using System.Threading.Tasks;
using MapR.Data.Models;
using System.Collections.Generic;

namespace MapR.Data.Stores {
    public interface IStoreMarkers {
        Task<MarkerModel> AddMarker(MarkerModel marker);
        Task UpdateMarker(MarkerModel marker);
        Task DeleteMarker(string markerId);
        Task<MarkerModel> GetMarker(string markerId);
        Task<IList<MarkerModel>> GetMarkers(string mapId);
    }
}
