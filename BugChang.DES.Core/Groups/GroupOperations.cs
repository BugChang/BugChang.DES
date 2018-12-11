using BugChang.DES.Core.Authorization.Operations;

namespace BugChang.DES.Core.Groups
{
    public class GroupOperations : IOperations
    {
        public const string Module = "Group";
        public string GetMenuUrl()
        {
            return "/Group/Index";
        }

        public string GetModuleName()
        {
            return Module;
        }

        public Operation Create => new Operation
        {
            Name = Operation.CreateName,
            Code = $"{Module}.{nameof(Create)}"
        };

        public Operation Edit => new Operation
        {
            Name = Operation.EditName,
            Code = $"{Module}.{nameof(Edit)}"
        };
        public Operation Delete => new Operation
        {
            Name = Operation.DeleteName,
            Code = $"{Module}.{nameof(Delete)}"
        };

        public Operation AssignDetail => new Operation()
        {
            Name = "分配单位",
            Code = $"{Module}.{nameof(AssignDetail)}"
        };
    }
}
