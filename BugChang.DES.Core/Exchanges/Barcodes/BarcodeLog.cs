using System;
using System.ComponentModel.DataAnnotations.Schema;
using BugChang.DES.Core.Authorization.Users;
using BugChang.DES.Core.Commons;
using BugChang.DES.Core.Departments;
using BugChang.DES.Core.Exchanges.ExchangeObjects;
using BugChang.DES.Core.Exchanges.Places;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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
        /// 条码子状态
        /// </summary>
        public EnumBarcodeSubStatus BarcodeSubStatus { get; set; }

        /// <summary>
        /// 前一个操作时间
        /// </summary>
        public DateTime LastOperationTime { get; set; }

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

        /// <summary>
        /// 清单是否同步
        /// </summary>
        public bool IsSynBill { get; set; }

        public bool IsDeleted { get; set; }

        [ForeignKey("CurrentObjectId")]
        public virtual ExchangeObject CurrentObject { get; set; }

        [ForeignKey("CurrentPlaceId")]
        public virtual Place CurrentPlace { get; set; }

        [ForeignKey("OperatorId")]
        public virtual User Operator { get; set; }


        [ForeignKey("DepartmentId")]
        public virtual Department Department { get; set; }

    }
}
