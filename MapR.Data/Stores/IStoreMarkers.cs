using System.Threading.Tasks;
using MapR.Data.Models;
using System.Collections.Generic;

namespace MapR.Data.Stores {
    public interface IStoreMarkers {
        Task<IAmAMarkerModel> AddMarker(IAmAMarkerModel marker);
        Task UpdateMarker(IAmAMarkerModel marker);
        Task DeleteMarker(string markerId);
        Task<IAmAMarkerModel> GetMarker(string markerId);
        Task<IList<IAmAMarkerModel>> GetMarkers(string mapId);
    }
}
