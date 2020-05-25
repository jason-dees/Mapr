
namespace MapR.Api.Models {
    public class MarkerModel : Data.Models.MarkerModel {
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
        public string IconUrl { get => $"/game/{GameId}/maps/{MapId}/markers/{Id}"; }
        public bool HasIcon { get; set; }
    }
}
