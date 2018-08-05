using BugChang.DES.Application.Commons;

namespace BugChang.DES.Application.Letters.Dtos
{
    public class LetterSendListDto : BaseDto
    {
        /// <summary>
        /// 条码号
        /// </summary>
        public string BarcodeNo { get; set; }

        /// <summary>
        /// 信封编号
        /// </summary>
        public string LetterNo { get; set; }

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
        /// 附加数据
        /// </summary>
        public string CustomData { get; set; }

        /// <summary>
        /// 收件人
        /// </summary>
        public string Receiver { get; set; }
    }
}
