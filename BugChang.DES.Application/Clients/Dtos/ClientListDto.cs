using BugChang.DES.Application.Commons;

namespace BugChang.DES.Application.Clients.Dtos
{
    public class ClientListDto : BaseDto
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        ///<summary>
        /// 设备码
        /// </summary>
        public string DeviceCode { get; set; }

        /// <summary>
        /// 交换场所
        /// </summary>
        public string PlaceName { get; set; }

        /// <summary>
        /// 客户端类型
        /// </summary>
        public string ClientType { get; set; }

        /// <summary>
        /// 默认首页
        /// </summary>
        public string HomePage { get; set; }
    }
}
