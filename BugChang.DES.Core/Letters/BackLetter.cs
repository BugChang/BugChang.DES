using System;
using System.ComponentModel.DataAnnotations.Schema;
using BugChang.DES.Core.Authorization.Users;
using BugChang.DES.Core.Commons;
using BugChang.DES.Core.Departments;

namespace BugChang.DES.Core.Letters
{
    public class BackLetter : BaseEntity<int>
    {
        /// <summary>
        /// 信件ID
        /// </summary>
        public int LetterId { get; set; }

        /// <summary>
        /// 操作单位ID
        /// </summary>
        public int OperationDepartmentId { get; set; }

        /// <summary>
        /// 操作人ID
        /// </summary>
        public int OperatorId { get; set; }

        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime OperationTime { get; set; }


        [ForeignKey("LetterId")]
        public virtual Letter Letter { get; set; }

        [ForeignKey("OperatorId")]
        public virtual User Operator { get; set; }

        [ForeignKey("OperationDepartmentId")]
        public virtual Department OperationDepartment { get; set; }
    }
}
