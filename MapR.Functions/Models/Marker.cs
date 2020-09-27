using MapR.Data.Models;

namespace MapR.Functions.Models {
    public class Marker : IAmAMarkerModel {

        public Marker(IAmAMarkerModel model) {
            Id = model.Id;
            GameId = model.GameId;
            MapId = model.MapId;
            X = model.X;
            Y = model.Y;
            Name = model.Name;
            Description = model.Description;
        }

        public string Id { get; set; }
        public string GameId { get; set; }
        public string MapId { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CustomCss { get; set; }
        public string ImageUri { get; set; }
        public byte[] ImageBytes { get; set; }
    }
}
