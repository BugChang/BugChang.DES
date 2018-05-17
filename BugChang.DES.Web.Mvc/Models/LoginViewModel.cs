using System.ComponentModel.DataAnnotations;

namespace BugChang.DES.Web.Mvc.Models
{
    public class LoginViewModel
    {
        [Required]
        [MaxLength(20)]
        public string UserName { get; set; }

        [Required]
        [MinLength(8)]
        public string Password { get; set; }

        [Required]
        public bool RememberMe { get; set; }
    }
}
