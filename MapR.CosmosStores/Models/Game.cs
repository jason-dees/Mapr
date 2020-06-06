using MapR.CosmosStores.Models;
using MapR.Data.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MapR.CosmosStores.Models {
    public class Game {

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string Owner { get; set; }
        public string Name { get; set; }
        public bool IsPrivate { get; set; }
        public DateTimeOffset LastPlayed { get; set; }
        public IList<Map> Maps { get; set; } = new List<Map>();

        public override string ToString() {
            return JsonConvert.SerializeObject(this);
        }
    }
}