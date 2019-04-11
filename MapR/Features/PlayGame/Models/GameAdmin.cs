using System.Collections.Generic;

namespace MapR.Features.PlayGame.Models {
    public class GameAdmin {
        public string UserName { get; set; }
        public string GameId { get; set; }
        public string GameName { get; set; }
        public List<GameMap> Maps { get; set; }
        public GameMap ActiveMap { get; set; }
    }

    public class GameMap {
        public string Name { get; set; }
        public byte[] ImageBytes { get; set; }
        public string Id { get; set; }
        public bool IsPrimary { get; set; }
    }
}