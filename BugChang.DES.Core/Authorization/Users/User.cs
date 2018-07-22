using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BugChang.DES.Core.Commons;
using BugChang.DES.Core.Departments;

namespace BugChang.DES.Core.Authorization.Users
{
    public class User : BaseEntity, ISoftDelete
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

        /// <summary>
        /// 所属部门Id
        /// </summary>
        public int DepartmentId { get; set; }

        /// <summary>
        /// 所属部门
        /// </summary>
        [ForeignKey("DepartmentId")]
        public virtual Department Department { get; set; }

        /// <summary>
        /// 用户角色关联列表
        /// </summary>
        public virtual IList<UserRole> UserRoles { get; set; }

        /// <summary>
        /// 删除标识
        /// </summary>
        public bool IsDeleted { get; set; }

        public void ChangePassword(string newPassword)
        {
            Password = newPassword;
        }
    }
}
