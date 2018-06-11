using System.ComponentModel.DataAnnotations;
using BugChang.DES.Application.Commons;

namespace BugChang.DES.Application.Roles.Dtos
{
    public class RoleEditDto : EditDto
    {
        [Required(ErrorMessage = "角色名称不允许为空"), MaxLength(20, ErrorMessage = "角色名称不能超过20字")]
        public string Name { get; set; }

        [MaxLength(300, ErrorMessage = "描述最多300字")]
        public string Description { get; set; }
    }
}
