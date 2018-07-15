using System;
using BugChang.DES.Core.Authorization.Operations;

namespace BugChang.DES.Core.Exchanges.Boxs
{
    public class BoxOperations : IOperations
    {
        private const string Module = "Box";
        public string GetMenuUrl()
        {
            return "/Box/Index";
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

        public Operation AssignObject => new Operation
        {
            Name = "分配流转对象",
            Code = $"{Module}.{nameof(AssignObject)}"
        };
    }
}
