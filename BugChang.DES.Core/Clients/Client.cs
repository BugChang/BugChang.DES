using BugChang.DES.Core.Commons;

namespace BugChang.DES.Core.Clients
{
    public class Client : BaseEntity, ISoftDelete
    {
        /// <summary>
        /// 设备码
        /// </summary>
        public string DeviceCode { get; set; }

        /// <summary>
        /// 交换场所
        /// </summary>
        public int PlaceId { get; set; }

        /// <summary>
        /// 客户端类型
        /// </summary>
        public EnumClientType ClientType { get; set; }  

        public bool IsDeleted { get; set; }
    }
}
