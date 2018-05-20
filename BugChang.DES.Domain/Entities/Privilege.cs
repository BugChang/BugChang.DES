using System.ComponentModel.DataAnnotations;
using BugChang.DES.Domain.VaulesObject;

namespace BugChang.DES.Domain.Entities
{
    /// <summary>
    /// 权限
    /// </summary>
    public class Privilege : BaseEntity
    {
        /// <summary>
        /// 权限类型
        /// </summary>
        [Required]
        public EnumPrivilegeType Type { get; set; }

        /// <summary>
        /// 资源标识
        /// </summary>
        public int ResourceId { get; set; }
    }
}
