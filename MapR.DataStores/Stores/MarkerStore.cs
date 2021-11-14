using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MapR.Data.Stores;
using MapR.DataStores.Extensions;
using MapR.DataStores.Models;

namespace MapR.DataStores.Stores {

	public class MarkerStore :  IStoreMarkers {
        readonly IMapper _mapper;
        readonly IAccessCloudTableData<MarkerModel> _cloudTableAccess;
        readonly IAccessCloudBlobData _cloudBlobAccess;

        public MarkerStore(IAccessCloudTableData<MarkerModel> cloudTableAccess, 
            IAccessCloudBlobData  cloudBlobData,
			IMapper mapper) {

            _cloudTableAccess = cloudTableAccess;
            _cloudBlobAccess = cloudBlobData;
            _mapper = mapper;
        }

        public async Task<Data.Models.IAmAMarkerModel> AddMarker(Data.Models.IAmAMarkerModel newMarker) {
            var marker = _mapper.Map<MarkerModel>(newMarker);
            marker.GenerateRandomId();
            marker.Id = "M" + marker.Id;

            if (marker.ImageBytes.Any()) {
                marker.ImageUri = await _cloudBlobAccess.UploadBlob(marker.ImageBlobName, marker.ImageBytes);
            }
            await _cloudTableAccess.InsertOrMerge(marker);

            return marker;
		}

		public async Task DeleteMarker(string markerId) {
            await _cloudTableAccess.Delete(markerId);
        }

        public async Task<Data.Models.IAmAMarkerModel> GetMarker(string markerId) {
            return await _cloudTableAccess.GetByRowKey(markerId);
        }

        public async Task<IList<Data.Models.IAmAMarkerModel>> GetMarkers(string mapId) {
            var markers = (await _cloudTableAccess.GetByPartitionKey(mapId)).Select(m => m as Data.Models.IAmAMarkerModel).ToList();
            //We never want image bytes when getting ALL markers, so clearing out.
            //This caused issues with signalr sending down all markers for a map
            for(var i = 0; i< markers.Count; i++) {
                (markers[i] as MarkerModel).ImageBytes = new byte[0];
            }
            return markers;
		}

		public async Task UpdateMarker(Data.Models.IAmAMarkerModel marker) {
            var currentMarker = await _cloudTableAccess.GetByRowKey(marker.Id);
			currentMarker.CustomCss = marker.CustomCss;
			currentMarker.Description = marker.Description;
			currentMarker.X = marker.X;
			currentMarker.Y = marker.Y;

            await _cloudTableAccess.InsertOrMerge(currentMarker);
        }
	}
}
