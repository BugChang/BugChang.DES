using BugChang.DES.Core.Commons;

namespace BugChang.DES.Core.Groups
{
    public class Group : BaseEntity,ISoftDelete
    {
        public string Name { get; set; }

        public EnumGroupType Type { get; set; }

        public string Description { get; set; }
        public bool IsDeleted { get; set; }
    }
}
