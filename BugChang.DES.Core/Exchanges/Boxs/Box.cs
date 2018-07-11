using System.ComponentModel.DataAnnotations.Schema;
using BugChang.DES.Core.Commons;
using BugChang.DES.Core.Exchanges.Places;

namespace BugChang.DES.Core.Exchanges.Boxs
{
    public class Box : BaseEntity, ISoftDelete
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
        /// 常驻提示信息
        /// </summary>
        public string PermanentMessage { get; set; }

        /// <summary>
        /// 场所ID
        /// </summary>
        public int PlaceId { get; set; }

        public bool IsDeleted { get; set; }


        [ForeignKey("PlaceId")]
        public virtual Place Place { get; set; }
    }
}
