using System;

namespace MapR.Data.Models {
    public interface GameModel {
		string Id { get; set; }
        string Owner { get; set; }
		string Name { get; set; }
		bool IsPrivate { get; set; }
        DateTimeOffset LastPlayed { get; set; }
	}
}