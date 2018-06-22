using BugChang.DES.Core.Commons;

namespace BugChang.DES.Core.Exchanges.Boxs
{
    public class Box : BaseEntity, ISoftDelete
    {
        /// <summary>
        /// 设备码
        /// </summary>
        public string DeviceCode { get; set; }

        /// <summary>
        /// 文件数量
        /// </summary>
        public int FileCount { get; set; }

        /// <summary>
        /// 有紧急文件
        /// </summary>
        public bool HasUrgentFile { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 提示信息
        /// </summary>
        public string Tips { get; set; }

        /// <summary>
        /// 场所ID
        /// </summary>
        public int PlaceId { get; set; }

        /// <summary>
        /// 优先级
        /// </summary>
        public int Order { get; set; }

        public bool IsDeleted { get; set; }
    }
}
