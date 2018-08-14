using System.ComponentModel.DataAnnotations.Schema;
using BugChang.DES.Core.Commons;
using BugChang.DES.Core.Letters;

namespace BugChang.DES.Core.Sortings
{
   public class SortingListDetail:BaseEntity<int>,ISoftDelete
    {
        public int SortingListId { get; set; }

        public int LetterId { get; set; }   

        public bool IsDeleted { get; set; }

        [ForeignKey("SortingListId")]
        public virtual SortingList SortingList { get; set; }

        [ForeignKey("LetterId")]
        public virtual Letter Letter { get; set; }
    }
}
