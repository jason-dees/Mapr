namespace MapR.Api.Models {
    public class MapModel : Data.Models.MapModel {
        public string Id { get; set; }
        public byte[] ImageBytes { get; set; }
        public string GameId { get; set; }
        public string ImageUri { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool IsPrimary { get; set; }
    }
}
