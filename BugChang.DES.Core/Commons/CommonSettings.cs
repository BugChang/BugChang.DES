namespace BugChang.DES.Core.Commons
{
    public class CommonSettings
    {
        /// <summary>
        /// 使用单位代码
        /// </summary>
        public string UseDepartmentCode { get; set; }

        /// <summary>
        /// 硬盘分区名称
        /// </summary>
        public string HardDiskPartition { get; set; }

        /// <summary>
        /// 收信部门（管理部门）
        /// </summary>
        public int ReceiveDepartmentId { get; set; }


        /// <summary>
        /// 系统使用单位
        /// </summary>
        public int UseDepartmentId { get; set; }


        /// <summary>
        /// 顶级交换场所ID
        /// </summary>
        public int RootPlaceId { get; set; }

        /// <summary>
        /// 读卡单位
        /// </summary>
        public string ReadCardDepartmentCode { get; set; }

        /// <summary>
        /// Referer
        /// </summary>
        public string Referer { get; set; }
    }
}
