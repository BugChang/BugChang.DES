using System.ComponentModel.DataAnnotations;

namespace BugChang.DES.Domain.Entities
{
    /// <summary>
    /// 角色
    /// </summary>
    public class Role : BaseEntity
    {
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
    }
}
