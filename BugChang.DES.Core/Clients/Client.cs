using System.ComponentModel.DataAnnotations.Schema;
using BugChang.DES.Core.Commons;
using BugChang.DES.Core.Exchanges.Places;

namespace BugChang.DES.Core.Clients
{
    public class Client : BaseEntity
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
        /// 交换场所
        /// </summary>
        public int PlaceId { get; set; }

        /// <summary>
        /// 客户端类型
        /// </summary>
        public EnumClientType ClientType { get; set; }

        /// <summary>
        /// 默认首页
        /// </summary>
        public string HomePage { get; set; }

        [ForeignKey("PlaceId")]
        public virtual Place Place { get; set; }
    }
}
