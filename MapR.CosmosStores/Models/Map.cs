using System.Collections;
using System.Collections.Generic;

namespace MapR.DataStores.Models {
    public class Map {
        public string Id { get; set; }
        public string GameId { get; set; }
        public string ImageUri { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool IsPrimary { get; set; }

        public IList<Marker> Markers { get; set; } = new List<Marker>();
    }
}
