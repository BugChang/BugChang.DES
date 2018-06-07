using System.Collections.Generic;

namespace BugChang.DES.Core.Commons
{
    public class PageResultModel<T>
    {
        public int Total { get; set; }

        public IList<T> Rows { get; set; }
    }
}
