using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;

namespace MapR.Features.PlayGame.Models {
    public class GamePlayer {

        public string GameName { get; set; }
        public string GameId { get; set; }
        public bool IsSignedIn { get; set; }
        public string UserName { get; set; }
        public IEnumerable<AuthenticationScheme> AuthenticationSchemes { get; set; }
    }
}
