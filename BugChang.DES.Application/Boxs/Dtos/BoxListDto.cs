using BugChang.DES.Application.Commons;

namespace BugChang.DES.Application.Boxs.Dtos
{
    public class BoxListDto : BaseDto
    {
        /// <summary>
        /// 设备码
        /// </summary>
        public string DeviceCode { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 文件数量
        /// </summary>
        public int FileCount { get; set; }

        /// <summary>
        /// 有紧急文件
        /// </summary>
        public bool HasUrgentFile { get; set; }

        /// <summary>
        /// 提示信息
        /// </summary>
        public string Tips { get; set; }

        /// <summary>
        /// 场所
        /// </summary>
        public int PlaceName { get; set; }

        /// <summary>
        /// 流转对象
        /// </summary>
        public int ObjectName { get; set; }

        /// <summary>
        /// 优先级
        /// </summary>
        public int Order { get; set; }
    }
}
