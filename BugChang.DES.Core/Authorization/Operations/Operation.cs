namespace BugChang.DES.Core.Authorization.Operations
{
    /// <summary>
    /// 操作
    /// </summary>
    public class Operation
    {
        public const string CreateName = "新增";
        public const string EditName = "修改";
        public const string DeleteName = "删除";

        /// <summary>
        /// 所属菜单的Url
        /// </summary>
        public string MenuUrl { get; set; }

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
