using BugChang.DES.Application.Commons;

namespace BugChang.DES.Application.Letters.Dtos
{
    public class ExchangeLogListDto
    {

        public int Id { get; set; }
        /// <summary>
        /// 条码号
        /// </summary>
        public string BarcodeNumber { get; set; }

        /// <summary>
        /// 条码状态
        /// </summary>
        public string BarcodeStatus { get; set; }

        /// <summary>
        /// 条码子状态
        /// </summary>
        public string BarcodeSubStatus { get; set; }

        /// <summary>
        /// 前一个操作时间
        /// </summary>
        public string LastOperationTime { get; set; }

        /// <summary>
        /// 操作时间
        /// </summary>
        public string OperationTime { get; set; }

        /// <summary>
        /// 操作单位
        /// </summary>
        public string DepartmentName { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        public string OperatorName { get; set; }

        /// <summary>
        /// 当前流转对象Id
        /// </summary>
        public string CurrentObjectName { get; set; }

        /// <summary>
        /// 当前场所
        /// </summary>
        public string CurrentPlaceName { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 清单是否同步
        /// </summary>
        public string IsSynBill { get; set; }

    }
}
