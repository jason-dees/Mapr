using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MapR.Data.Stores;
using MapR.DataStores.Extensions;
using MapR.DataStores.Models;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;

namespace MapR.DataStores.Stores {

	public class MarkerStore : ImageStore<MarkerModel>, IStoreMarkers {
        readonly IMapper _mapper;

        public MarkerStore(CloudTable markerTable,
            CloudBlobContainer iconContainer, 
            IMapper mapper) 
            : base(markerTable, iconContainer) {
            _mapper = mapper;
        }

        public async Task<Data.Models.MarkerModel> AddMarker(Data.Models.MarkerModel newMarker) {
            var marker = _mapper.Map<MarkerModel>(newMarker);
            marker.GenerateRandomId();
            marker.Id = "M" + marker.Id;

            if (marker.ImageBytes.Any()) {
                marker.ImageUri = await UploadImage(marker.ImageBlobName, marker.ImageBytes);
            }
            await InsertOrMerge(marker);

            return marker;
		}

		public async Task DeleteMarker(string markerId) {
            await Delete(markerId);
        }

        public async Task<Data.Models.MarkerModel> GetMarker(string markerId) {
            return await GetByRowKey(markerId);
        }

        public async Task<IList<Data.Models.MarkerModel>> GetMarkers(string mapId) {
            var markers = (await GetByPartitionKey(mapId)).Select(m => m as Data.Models.MarkerModel).ToList();
            //We never want image bytes when getting ALL markers, so clearing out.
            //This caused issues with signalr sending down all markers for a map
            for(var i = 0; i< markers.Count; i++) {
                markers[i].ImageBytes = new byte[0];
            }
            return markers;
		}

		public async Task UpdateMarker(Data.Models.MarkerModel marker) {
            var currentMarker = await GetByRowKey(marker.Id);
			currentMarker.CustomCss = marker.CustomCss;
			currentMarker.Description = marker.Description;
			currentMarker.X = marker.X;
			currentMarker.Y = marker.Y;

            var cacheMarker = _cache.FirstOrDefault(m => m.Id == marker.Id);
            cacheMarker.CustomCss = marker.CustomCss;
            cacheMarker.Description = marker.Description;
            cacheMarker.X = marker.X;
            cacheMarker.Y = marker.Y;

            await InsertOrMerge(currentMarker);
        }
	}
}
