namespace BugChang.DES.Application.Letters.Dtos
{
    public class LetterReceiveBarcodeDto
    {
        /// <summary>
        /// 条码号
        /// </summary>
        public string BarcodeNo { get; set; }

        /// <summary>
        /// 信封号
        /// </summary>
        public string LetterNo { get; set; }

        public string OldBarcodeNo { get; set; }

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

        public string OldSendDepartmentName { get; set; }

        /// <summary>
        /// 市机码
        /// </summary>
        public string ShiJiCode { get; set; }

        /// <summary>
        /// 附加数据
        /// </summary>
        public string CustomData { get; set; }

        public string PrintDate { get; set; }

        public string Receiver { get; set; }
    }
}
