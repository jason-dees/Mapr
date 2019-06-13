using System;
using System.Collections.Generic;
using MapR.Data.Models;

namespace MapR.Functions.Models {
    public class Game : GameModel {
        public Game(GameModel model) {
            Id = model.Id;
            Owner = model.Owner;
            Name = model.Name;
            IsPrivate = model.IsPrivate;
            LastPlayed = model.LastPlayed;
        }

        public string Id { get; set; }
        public string Owner { get; set; }
        public string Name { get; set; }
        public bool IsPrivate { get; set; }
        public DateTimeOffset LastPlayed { get; set; }
        public string PrimaryMapId { get; set; }
        public IEnumerable<Map> Maps { get; set; }
    }
}