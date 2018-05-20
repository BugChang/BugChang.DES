using System.ComponentModel.DataAnnotations;

namespace BugChang.DES.Domain.Entities
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

        public int ParentId { get; set; }
    }
}
