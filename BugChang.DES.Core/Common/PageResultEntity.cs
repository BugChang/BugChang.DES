using System.Collections.Generic;

namespace BugChang.DES.Core.Common
{
    public class PageResultEntity<T>
    {
        public int Total { get; set; }

        public IList<T> Rows { get; set; }
    }
}
