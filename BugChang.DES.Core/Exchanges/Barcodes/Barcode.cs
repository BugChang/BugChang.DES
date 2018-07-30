using BugChang.DES.Core.Commons;

namespace BugChang.DES.Core.Exchanges.Barcodes
{
    public class Barcode : BaseEntity, ISoftDelete
    {
        /// <summary>
        /// 条码号
        /// </summary>
        public string BarcodeNo { get; set; }

        /// <summary>
        /// 条码类型
        /// </summary>
        public EnumBarcodeType BarcodeType { get; set; }

        /// <summary>
        /// 条码状态
        /// </summary>
        public EnumBarcodeStatus Status { get; set; }

        /// <summary>
        /// 条码子状态
        /// </summary>
        public EnumBarcodeSubStatus SubStatus { get; set; }

        /// <summary>
        /// 条码来源
        /// </summary>
        public EnumBarcodeSouce Souce { get; set; }

        /// <summary>
        /// 条码实体
        /// </summary>
        public EnumBarcodeEntity Entity { get; set; }

        /// <summary>
        /// 当前流转对象Id
        /// </summary>
        public int CurrentObjectId { get; set; }

        /// <summary>
        /// 附加数据
        /// </summary>
        public string CustomData { get; set; }

        public bool IsDeleted { get; set; }

        /// <summary>
        /// 解析条码类型
        /// </summary>
        public EnumBarcodeType AnalysisBarcodeType(string barcodeNo)
        {
            switch (barcodeNo.Length)
            {
                case 12:
                    return EnumBarcodeType.机要通信12位;

                case 26:
                    return EnumBarcodeType.国办26位;

                case 33:
                    return EnumBarcodeType.安全部33位;

                default:
                    return EnumBarcodeType.未知条码;
            }

        }

        /// <summary>
        /// 解析条码实体
        /// </summary>
        /// <param name="barcodeType"></param>
        /// <returns></returns>
        public EnumBarcodeEntity AnalysisBarcodeEntity(EnumBarcodeType barcodeType)
        {
            switch (barcodeType)
            {
                case EnumBarcodeType.未知条码:
                    break;
                case EnumBarcodeType.国办26位:
                case EnumBarcodeType.安全部33位:
                case EnumBarcodeType.机要通信12位:
                    return EnumBarcodeEntity.信件;
            }

            return EnumBarcodeEntity.未知;
        }
    }
}
