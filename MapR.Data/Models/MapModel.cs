using System.Collections;
using System.Collections.Generic;

namespace MapR.Data.Models {
    public class MapModel { 

        public string Id { get; set; }
        public string GameId { get; set; }
        public string ImageUri { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool IsPrimary { get; set; }

        public IList<MarkerModel> Markers { get; set; }
    }
}