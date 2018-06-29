namespace BugChang.DES.Core.Exchanges.Barcodes
{
    public class BarcodeRule
    {
        /// <summary>
        /// 规则名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 条码类型
        /// </summary>
        public EnumBarcodeType EnumBarcodeType { get; set; }

        /// <summary>
        /// 不登记投箱
        /// </summary>
        public bool NoRegisterSend { get; set; }

        /// <summary>
        /// 是否解析条码中单位
        /// </summary>
        public bool IsAnalysisDepartment { get; set; }

        /// <summary>
        /// 解析单位级别
        /// </summary>
        public int AnalysisDepartmentLevel { get; set; }
    }
}
