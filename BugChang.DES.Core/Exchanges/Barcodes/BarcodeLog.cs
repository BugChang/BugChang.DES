using System;
using BugChang.DES.Core.Commons;

namespace BugChang.DES.Core.Exchanges.Barcodes
{
    public class BarcodeLog : BaseEntity<int>, ISoftDelete
    {

        /// <summary>
        /// 条码号
        /// </summary>
        public string BarcodeNumber { get; set; }

        /// <summary>
        /// 条码状态
        /// </summary>
        public EnumBarcodeStatus BarcodeStatus { get; set; }

        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime OperationTime { get; set; }

        /// <summary>
        /// 操作单位
        /// </summary>
        public int DepartmentId { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        public int? OperatorId { get; set; }

        /// <summary>
        /// 当前流转对象Id
        /// </summary>
        public int CurrentObjectId { get; set; }

        /// <summary>
        /// 当前场所
        /// </summary>
        public int CurrentPlaceId { get; set; }


        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }


        public bool IsDeleted { get; set; }
    }
}
