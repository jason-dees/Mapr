using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MapR.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MapR.Pages
{
    public class IndexModel : PageModel
    {

		readonly SignInManager<ApplicationUser> _signInManager;
		public IndexModel(SignInManager<ApplicationUser> signInManager) {
			_signInManager = signInManager;
		}

        public void OnGet()
        {

		}

		public override async Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context) {
			AuthenticationSchemes = await _signInManager.GetExternalAuthenticationSchemesAsync();
		}


		public IEnumerable<AuthenticationScheme> AuthenticationSchemes { get; set; }

    }
}
