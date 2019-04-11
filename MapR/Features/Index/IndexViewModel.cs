using System.Collections.Generic;
using MapR.Features.Index.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MapR.Features.Index {
    public class IndexViewModel : PageModel {

        public IEnumerable<AuthenticationScheme> AuthenticationSchemes { get; set; }

        public bool IsSignedIn { get; set; }

        public string UserName { get; set; }

        public List<YourGame> YourGames { get; set; }
    }
}
