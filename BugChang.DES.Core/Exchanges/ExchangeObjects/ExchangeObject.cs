using System.ComponentModel.DataAnnotations;
using BugChang.DES.Core.Commons;

namespace BugChang.DES.Core.Exchanges.ExchangeObjects
{
    public class ExchangeObject : BaseEntity, ISoftDelete
    {
        [Required]
        [MaxLength(50, ErrorMessage = "名称长度最多50个字符")]
        public string Name { get; set; }

        [Required]
        public EnumObjectType ObjectType { get; set; }

        [Required]
        public int Value { get; set; }

        [Required]
        public string ValueText { get; set; }

        public bool IsDeleted { get; set; }
    }
}
