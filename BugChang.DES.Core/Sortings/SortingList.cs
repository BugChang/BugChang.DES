using BugChang.DES.Core.Commons;
using BugChang.DES.Core.Exchanges.Channel;

namespace BugChang.DES.Core.Sortings
{
    public class SortingList : BaseEntity, ISoftDelete
    {
        public string ListNo { get; set; }

        public EnumChannel Channel { get; set; }

        public bool IsDeleted { get; set; }
    }
}
