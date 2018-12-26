using System;
using BugChang.DES.Application.Commons;
using BugChang.DES.Core.Letters;

namespace BugChang.DES.Application.Letters.Dtos
{
    public class LetterSendEditDto : EditDto
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
        /// 原条码号
        /// </summary>
        public string OldBarcodeNo { get; set; }
    }
}
