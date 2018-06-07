using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BugChang.DES.Core.Authorization.Users;
using BugChang.DES.Core.Commons;

namespace BugChang.DES.Core.Authorization.Roles
{
    /// <summary>
    /// 角色
    /// </summary>
    public class Role : BaseEntity
    {

        public const string SysAdmin = "系统管理员";
        public const string SecAdmin = "安全管理员";
        public const string AudAdmin = "审计管理员";

        /// <summary>
        /// 角色名称
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string Name { get; set; }

        /// <summary>
        /// 角色描述
        /// </summary>
        [MaxLength(500)]
        public string Description { get; set; }

        public IList<UserRole> UserRoles { get; set; }
    }
}
