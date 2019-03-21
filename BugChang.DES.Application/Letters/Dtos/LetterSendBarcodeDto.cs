using System;
using System.Collections.Generic;
using System.Text;

namespace BugChang.DES.Application.Letters.Dtos
{
    public class LetterSendBarcodeDto
    {
        /// <summary>
        /// 条码号
        /// </summary>
        public string BarcodeNo { get; set; }

        /// <summary>
        /// 原始条码号
        /// </summary>
        public string OldBarcodeNo { get; set; }

        /// <summary>
        /// 信封号
        /// </summary>
        public string LetterNo { get; set; }

        /// <summary>
        /// 密级
        /// </summary>
        public string SecretLevel { get; set; }

        /// <summary>
        /// 紧急程度
        /// </summary>
        public string UrgencyLevel { get; set; }

        /// <summary>
        /// 限时时间
        /// </summary>
        public string UrgencyTime { get; set; }

        /// <summary>
        /// 收件单位
        /// </summary>
        public string ReceiveDepartmentName { get; set; }

        /// <summary>
        /// 发件单位
        /// </summary>
        public string SendDepartmentName { get; set; }

        /// <summary>
        /// 原发单位
        /// </summary>
        public string OldSendDepartmentName { get; set; }

        /// <summary>
        /// 打印日期
        /// </summary>
        public string PrintDate { get; set; }

        /// <summary>
        /// 收件人
        /// </summary>
        public string Receiver { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }


        /// <summary>
        /// 箱号
        /// </summary>
        public string BoxNo { get; set; }
    }
}
