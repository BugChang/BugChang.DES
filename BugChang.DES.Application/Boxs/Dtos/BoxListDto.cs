using BugChang.DES.Application.Commons;

namespace BugChang.DES.Application.Boxs.Dtos
{
    public class BoxListDto : BaseDto
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
        /// 提示信息
        /// </summary>
        public string PermanentMessage { get; set; }

        /// <summary>
        /// 场所
        /// </summary>
        public string PlaceName { get; set; }

        public bool Enabled { get; set; }

        public bool HasUrgent { get; set; }

        public int FileCount { get; set; }

        public bool IsTwoLock { get; set; }
    }
}
