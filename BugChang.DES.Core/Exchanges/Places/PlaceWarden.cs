using BugChang.DES.Core.Commons;

namespace BugChang.DES.Core.Exchanges.Places
{
    /// <summary>
    /// 交换场所管理员
    /// </summary>
    public class PlaceWarden : BaseEntity<int>, ISoftDelete
    {
        /// <summary>
        /// 交换场所ID
        /// </summary>
        public int PlaceId { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId { get; set; }

        public bool IsDeleted { get; set; }
    }
}
