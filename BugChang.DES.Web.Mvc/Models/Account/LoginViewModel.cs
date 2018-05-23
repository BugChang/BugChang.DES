using System.ComponentModel.DataAnnotations;

namespace BugChang.DES.Web.Mvc.Models.Account
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "用户名不允许为空")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "密码不能为空")]
        public string Password { get; set; }

        [Required]
        public bool RememberMe { get; set; }
    }
}
