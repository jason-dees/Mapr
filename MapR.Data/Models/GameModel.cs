using System;
using System.Collections.Generic;

namespace MapR.Data.Models {
    public class GameModel {
		public string Id { get; set; }
        public string Owner { get; set; }
		public string Name { get; set; }
		public bool IsPrivate { get; set; }
        public DateTimeOffset LastPlayed { get; set; }

		public IList<MapModel> Maps { get; set; }
	}
}