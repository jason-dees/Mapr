using System.Collections.Generic;
using System.Linq;

namespace MapR.Features.PlayGame.Models {
    public class GameAdmin {
        public string UserName { get; set; }
        public string GameId { get; set; }
        public string GameName { get; set; }
        public List<GameMap> Maps { get; set; }
        public GameMap ActiveMap { get => Maps.FirstOrDefault(m => m.IsPrimary); }
    }

    public class GameMap {
        public string Name { get; set; }
        public string Id { get; set; }
        public bool IsPrimary { get; set; }
    }
}