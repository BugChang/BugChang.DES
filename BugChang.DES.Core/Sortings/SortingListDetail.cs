using System;
using System.Collections.Generic;
using System.Text;
using BugChang.DES.Core.Commons;

namespace BugChang.DES.Core.Sortings
{
   public class SortingListDetail:BaseEntity<int>,ISoftDelete
    {
        public int SortingListId { get; set; }

        public bool IsDeleted { get; set; }
    }
}
