using MapR.Data.Models;
using System;

namespace MapR.Api.Models {
    public class GameModel : IAmAGameModel {
        public string Id { get; set; }
        public string Owner { get; set; }
        public string Name { get; set; }
        public bool IsPrivate { get; set; }
        public DateTimeOffset LastPlayed { get; set; }
    }
}
