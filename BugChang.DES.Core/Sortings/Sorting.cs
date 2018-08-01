using BugChang.DES.Core.Commons;
using BugChang.DES.Core.Exchanges.Channel;

namespace BugChang.DES.Core.Sortings
{
    public class Sorting : BaseEntity<int>
    {

        /// <summary>
        /// 分拣渠道
        /// </summary>
        public EnumChannel Channel { get; set; }
        /// <summary>
        /// 条码号
        /// </summary>
        public string BarcodeNo { get; set; }
        /// <summary>
        /// 已分拣
        /// </summary>
        public bool Sorted { get; set; }
    }
}
