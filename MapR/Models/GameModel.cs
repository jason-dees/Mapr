using System;
namespace MapR.Models {
    public class GameModel : Data.Models.GameModel {
        public string Id { get; set; }
        public string Owner { get; set; }
        public string Name { get; set; }
        public bool IsPrivate { get; set; }
        public DateTimeOffset LastPlayed { get; set; }
    }
}
