using System.ComponentModel.DataAnnotations;
using BugChang.DES.Application.Commons;

namespace BugChang.DES.Application.Users.Dtos
{
    public class UserEditDto : EditDto
    {
        [Required(ErrorMessage = "所属机构不能为空")]
        public int DepartmentId { get; set; }

        [Required]
        [MaxLength(10, ErrorMessage = "用户名长度不能超过10位")]
        public string UserName { get; set; }

        public string Password { get; set; }

        [Required]
        [MaxLength(10, ErrorMessage = "姓名长度不能超过10位")]
        public string DisplayName { get; set; }

        public int Enabled { get; set; }

        public string UsbKeyNo { get; set; }

        [Phone]
        public string Phone { get; set; }

        public string Tel { get; set; }
    }
}
