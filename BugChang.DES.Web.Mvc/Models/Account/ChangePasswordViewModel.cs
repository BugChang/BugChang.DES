using System.ComponentModel.DataAnnotations;

namespace BugChang.DES.Web.Mvc.Models.Account
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "用户ID无效")]
        public int Id { get; set; }

        [Required(ErrorMessage = "用户名不能为空")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "原密码不能为空")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "新密码不能为空")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "两次密码输入不一致")]
        public string ConfirmPassword { get; set; }
    }
}
