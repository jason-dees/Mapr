using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MapR.CosmosStores.Models {
    public class MapRUser : IdentityUser {
        public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }
        [JsonProperty(PropertyName = "id")]
        public string NameIdentifier { get; set; }

        public override string ToString() {
            return JsonConvert.SerializeObject(this);
        }
    }
}
