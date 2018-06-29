using BugChang.DES.Core.Commons;

namespace BugChang.DES.Core.Exchanges.Barcodes
{
    public class Barcode : BaseEntity, ISoftDelete
    {
        /// <summary>
        /// 条码号
        /// </summary>
        public string BarcodeNumber { get; set; }

        /// <summary>
        /// 条码类型
        /// </summary>
        public EnumBarcodeType BarcodeType { get; set; }

        /// <summary>
        /// 条码状态
        /// </summary>
        public EnumBarcodeStatus BarcodeStatus { get; set; }

        /// <summary>
        /// 是否退件
        /// </summary>
        public bool IsBack { get; set; }

        /// <summary>
        /// 是否取消
        /// </summary>
        public bool IsCancel { get; set; }
        
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 解析条码类型
        /// </summary>
        public void AnalysisBarcodeType()
        {
            switch (BarcodeNumber.Length)
            {
                case 12:
                    BarcodeType = EnumBarcodeType.机要通信12位;
                    break;
                case 26:
                    BarcodeType = EnumBarcodeType.国办26位;
                    break;
                case 33:
                    BarcodeType = EnumBarcodeType.安全部33位;
                    break;
                default:
                    BarcodeType = EnumBarcodeType.未知条码;
                    break;
            }
        }
    }
}
