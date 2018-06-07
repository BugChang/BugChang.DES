using BugChang.DES.Core.Commons;

namespace BugChang.DES.Core.Authorization.Operations
{
    /// <summary>
    /// 操作
    /// </summary>
    public class Operation : BaseEntity
    {
        /// <summary>
        /// 操作名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 操作代码
        /// </summary>
        public string Code { get; set; }
    }
}
