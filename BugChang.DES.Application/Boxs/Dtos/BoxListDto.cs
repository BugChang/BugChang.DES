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
        /// 提示信息
        /// </summary>
        public string PermanentMessage { get; set; }

        /// <summary>
        /// 场所
        /// </summary>
        public string PlaceName { get; set; }
    }
}
