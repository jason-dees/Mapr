using System.ComponentModel.DataAnnotations;

namespace MapR.Identity.Models {
    public class ExternalLoginViewModel {

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
