using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MapR.Features.Index {
    public class IndexViewModel : PageModel {

        public IEnumerable<AuthenticationScheme> AuthenticationSchemes { get; set; }

        public bool IsSignedIn { get; set; }
    }
}
