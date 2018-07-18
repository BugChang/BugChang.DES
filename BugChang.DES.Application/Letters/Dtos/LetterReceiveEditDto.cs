using System;
using BugChang.DES.Application.Commons;

namespace BugChang.DES.Application.Letters.Dtos
{
    public class ReceiveLetterEditDto : EditDto
    {
        /// <summary>
        /// 条码号
        /// </summary>
        public string BarcodeNo { get; set; }

        /// <summary>
        /// 原条码号
        /// </summary>
        public string OldBarcodeNo { get; set; }

        /// <summary>
        /// 密级
        /// </summary>
        public int SecretLevel { get; set; }

        /// <summary>
        /// 紧急程度
        /// </summary>
        public int UrgencyLevel { get; set; }

        /// <summary>
        /// 限时时间
        /// </summary>
        public DateTime? UrgencyTime { get; set; }

        /// <summary>
        /// 收件单位
        /// </summary>
        public int ReceiveDepartmentId { get; set; }

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
    }
}
