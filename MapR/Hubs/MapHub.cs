using MapR.Extensions;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MapR.Hubs {
    public class MapHub : Hub {

        static List<MapMarker> _mapMarkers = new List<MapMarker>();

        public async Task SendMapMarker(MapMarker mapMarker) {
            await Clients.All.SendAsync("SetMapMarker", mapMarker);

            if (_mapMarkers.Contains(mapMarker)) { _mapMarkers.Replace(mapMarker, mapMarker); }
            _mapMarkers.Add(mapMarker);
        }

        public async Task SendAllMapMarkers() {

			await Clients.All.SendAsync("SetAllMapMarkers", _mapMarkers);
        }
    }

    public class MapMarker {
        public string Id { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public override bool Equals(object obj) {
            if(typeof(MapMarker) != obj.GetType()) { return false; }

            return Id == ((MapMarker)obj).Id;
        }

        public override int GetHashCode() {
            return HashCode.Combine(Id, X, Y);
        }
    }
}
