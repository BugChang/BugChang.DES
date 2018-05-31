using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BugChang.DES.Core.Common;

namespace BugChang.DES.Core.Authorization.Menus
{
    /// <inheritdoc />
    /// <summary>
    /// 菜单
    /// </summary>
    public class Menu : BaseEntity
    {
        /// <summary>
        /// 名称
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string Name { get; set; }

        [MaxLength(200)]
        public string Url { get; set; }

        [MaxLength(50)]
        public string Icon { get; set; }

        [MaxLength(200, ErrorMessage = "菜单描述不能多于200个字")]
        public string Description { get; set; }

        public int? ParentId { get; set; }

        public virtual IList<Menu> Items { get; set; }

        [ForeignKey("ParentId")]
        public virtual Menu Parent { get; set; }
    }
}
