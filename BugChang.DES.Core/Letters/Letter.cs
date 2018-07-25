using System;
using System.ComponentModel.DataAnnotations.Schema;
using BugChang.DES.Core.Commons;
using BugChang.DES.Core.Departments;
using BugChang.DES.Core.SecretLevels;
using BugChang.DES.Core.UrgentLevels;

namespace BugChang.DES.Core.Letters
{
    public class Letter : BaseEntity, ISoftDelete
    {
        /// <summary>
        /// 条码号
        /// </summary>
        public string BarcodeNo { get; set; }

        /// <summary>
        /// 信件类型
        /// </summary>
        public EnumLetterType LetterType { get; set; }  

        /// <summary>
        /// 原条码号
        /// </summary>
        public string OldBarcodeNo { get; set; }

        /// <summary>
        /// 密级
        /// </summary>
        public EnumSecretLevel SecretLevel { get; set; }

        /// <summary>
        /// 紧急程度
        /// </summary>
        public EnumUrgentLevel UrgencyLevel { get; set; }

        /// <summary>
        /// 现实时间
        /// </summary>
        public DateTime? UrgencyTime { get; set; }

        /// <summary>
        /// 收件单位
        /// </summary>
        public int ReceiveDepartmentId { get; set; }

        /// <summary>
        /// 收件人
        /// </summary>
        public string Receiver { get; set; }

        /// <summary>
        /// 发件单位
        /// </summary>
        public int SendDepartmentId { get; set; }

        /// <summary>
        /// 附加数据
        /// </summary>
        public string CustomData { get; set; }

        /// <summary>
        /// 原发单位名称
        /// </summary>
        public string OldSendDepartmentName { get; set; }

        public bool IsDeleted { get; set; }

        [ForeignKey("SendDepartmentId")]
        public virtual Department SendDepartment { get; set; }

        [ForeignKey("ReceiveDepartmentId")]
        public virtual Department ReceiveDepartment { get; set; }
    }
}
