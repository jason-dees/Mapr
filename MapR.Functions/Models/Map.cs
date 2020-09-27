using System.Collections.Generic;
using MapR.Data.Models;

namespace MapR.Functions.Models {
    public class Map : IAmAMapModel {
        public Map(IAmAMapModel model) {
            Id = model.Id;
            GameId = model.GameId;
            Name = model.Name;
            IsActive = model.IsActive;
            IsPrimary = model.IsPrimary;
        }

        public string Id { get; set; }
        public byte[] ImageBytes { get; set; }
        public string GameId { get; set; }
        public string ImageUri { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool IsPrimary { get; set; }
        public IEnumerable<Marker> Markers { get; set; }
    }
}