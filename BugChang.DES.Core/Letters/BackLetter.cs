using System;
using BugChang.DES.Core.Commons;

namespace BugChang.DES.Core.Letters
{
    public class BackLetter : BaseEntity<int>
    {
        /// <summary>
        /// 信件ID
        /// </summary>
        public int LetterId { get; set; }

        /// <summary>
        /// 操作人ID
        /// </summary>
        public int OperatorId { get; set; }

        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime OperationTime { get; set; }
    }
}
