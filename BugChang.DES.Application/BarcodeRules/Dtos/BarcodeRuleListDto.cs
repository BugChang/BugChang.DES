using BugChang.DES.Application.Commons;

namespace BugChang.DES.Application.BarcodeRules.Dtos
{
    public class BarcodeRuleListDto : BaseDto
    {
        /// <summary>
        /// 规则名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 条码类型
        /// </summary>
        public string BarcodeType { get; set; }

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
        /// 0：全部解析；>0解析至具体级别
        /// </summary>
        public int AnalysisDepartmentLevel { get; set; }
    }
}
