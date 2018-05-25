using System.ComponentModel.DataAnnotations;
using BugChang.DES.Core.Common;

namespace BugChang.DES.Core.Authorization.Powers
{
    /// <summary>
    /// 权限
    /// </summary>
    public class Power : BaseEntity
    {
        /// <summary>
        /// 权限类型
        /// </summary>
        [Required]
        public PowerType Type { get; set; }

        /// <summary>
        /// 资源标识
        /// </summary>
        public int ResourceId { get; set; }
    }
}
