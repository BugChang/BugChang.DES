using BugChang.DES.Core.Commons;

namespace BugChang.DES.Core.Sortings
{
   public class SortingListDetail:BaseEntity<int>,ISoftDelete
    {
        public int SortingListId { get; set; }

        public int LetterId { get; set; }   

        public bool IsDeleted { get; set; }
    }
}
