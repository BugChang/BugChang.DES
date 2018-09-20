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

        public int UserId { get; set; }
        public int DepartmentId { get; set; }
        public int? ParentId { get; set; }

        public int PlaceId { get; set; }
    }

    public class PageSearchDetailModel : PageSearchCommonModel
    {
        public DateTime? BeginTime { get; set; }

        public DateTime? EndTime { get; set; }

        public void SetTimeValue(string beginTime, string endTime)
        {
            if (string.IsNullOrWhiteSpace(beginTime))
            {
                BeginTime = DateTime.Now.AddDays(-7).Date;
            }
            else
            {
                BeginTime = Convert.ToDateTime(beginTime);
            }

            if (string.IsNullOrWhiteSpace(endTime))
            {
                EndTime = DateTime.Now.AddDays(1).Date;
            }
            else
            {
                EndTime = Convert.ToDateTime(endTime).AddDays(1);
            }

        }
    }
}
