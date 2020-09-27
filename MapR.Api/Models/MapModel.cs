using MapR.Data.Models;

namespace MapR.Api.Models {
    public class MapModel : IAmAMapModel {
        public string Id { get; set; }
        public string GameId { get; set; }
        public string ImageUri { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool IsPrimary { get; set; }
    }
}
