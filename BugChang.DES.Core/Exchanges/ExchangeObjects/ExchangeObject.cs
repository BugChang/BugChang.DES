using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BugChang.DES.Core.Commons;

namespace BugChang.DES.Core.Exchanges.ExchangeObjects
{
    public class ExchangeObject : BaseEntity, ISoftDelete
    {

        /// <summary>
        /// 流转对象名称
        /// </summary>
        [Required]
        [MaxLength(50, ErrorMessage = "名称长度最多50个字符")]
        public string Name { get; set; }


        /// <summary>
        /// 流转对象类型
        /// </summary>
        [Required]
        public EnumObjectType ObjectType { get; set; }

        /// <summary>
        /// 流转对象值
        /// </summary>
        [Required]
        public int Value { get; set; }

        /// <summary>
        /// 流转对象文本
        /// </summary>
        [Required]
        public string ValueText { get; set; }

        /// <summary>
        /// 上级流转对象
        /// </summary>
        public int? ParentId { get; set; }

        /// <summary>
        /// 虚拟流转对象
        /// （无实质性流转介质）
        /// </summary>
        public bool IsVirtual { get; set; }

        public bool IsDeleted { get; set; }

        [ForeignKey("ParentId")]
        public virtual ExchangeObject Parent { get; set; }
    }
}
