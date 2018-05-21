using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BugChang.DES.Core.Authorization.Roles;

namespace BugChang.DES.Core.Authorization.Users
{
    public class User : BaseEntity
    {

        public const string DefaultPassword = "123qwe";

        /// <summary>
        /// 用户名
        /// </summary>
        [Required]
        [MinLength(3), MaxLength(20)]
        public string UserName { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        [Required]
        [MinLength(3), MaxLength(50)]
        public string DisplayName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 登录错误次数
        /// </summary>
        public int LoginErrorCount { get; set; }

        /// <summary>
        /// 是否锁定
        /// </summary>
        public bool Locked { get; set; }

        /// <summary>
        /// 移动电话
        /// </summary>
        [Phone]
        public string Phone { get; set; }

        /// <summary>
        /// 固定电话
        /// </summary>
        [Phone]
        public string Tel { get; set; }


        public IList<UserRole> UserRoles { get; set; }

    }
}
