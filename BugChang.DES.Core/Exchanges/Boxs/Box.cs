using System.ComponentModel.DataAnnotations.Schema;
using BugChang.DES.Core.Commons;
using BugChang.DES.Core.Exchanges.Places;

namespace BugChang.DES.Core.Exchanges.Boxs
{
    public class Box : BaseEntity, ISoftDelete
    {
        /// <summary>
        /// 正面BN号
        /// </summary>
        public string FrontBn { get; set; }

        /// <summary>
        /// 背面BN号
        /// </summary>
        public string BackBn { get; set; }

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

        /// <summary>
        /// 是否双面锁
        /// </summary>
        public bool IsTwoLock { get; set; }

        /// <summary>
        /// 启用状态
        /// </summary>
        public bool Enabled { get; set; }

        public bool HasUrgent { get; set; }

        public int FileCount { get; set; }


        [ForeignKey("PlaceId")]
        public virtual Place Place { get; set; }
    }
}
