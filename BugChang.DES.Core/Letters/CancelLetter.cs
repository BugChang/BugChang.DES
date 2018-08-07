using System;
using BugChang.DES.Core.Commons;

namespace BugChang.DES.Core.Letters
{
    public class CancelLetter:BaseEntity<int>
    {
        /// <summary>
        /// 信件Id
        /// </summary>
        public int LetterId { get; set; }

        /// <summary>
        /// 申请人Id
        /// </summary>
        public int ApplicantId { get; set; }

        /// <summary>
        /// 操作人Id
        /// </summary>

        public int OperatorId { get; set; }

        /// <summary>
        /// 操作部门ID
        /// </summary>
        public int OperationDepartmentId { get; set; }

        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime OperationTime { get; set; }
    }
}
