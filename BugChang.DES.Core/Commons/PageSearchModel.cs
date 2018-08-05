using System;

namespace BugChang.DES.Core.Commons
{

    public abstract class PageSearchModelBase
    {
        public int Skip { get; set; }

        public int Take { get; set; }

    }

    public class PageSearchCommonModel : PageSearchModelBase
    {
        public string Keywords { get; set; }

        public int DepartmentId { get; set; }
        public int? ParentId { get; set; }
    }

    public class PageSearchDetailModel : PageSearchModelBase
    {
        public DateTime? BeginTime { get; set; }

        public DateTime? EndTime { get; set; }

        public void SetTimeValue(string beginTime,string endTime)
        {
            if (string.IsNullOrWhiteSpace(beginTime))
            {
                BeginTime = null;
            }
            else
            {
                BeginTime = Convert.ToDateTime(beginTime);
            }

            if (string.IsNullOrWhiteSpace(endTime))
            {
                EndTime = null;
            }
            else
            {
                EndTime = Convert.ToDateTime(endTime);
            }

        }
    }
}
