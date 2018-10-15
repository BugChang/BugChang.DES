using BugChang.DES.Core.Authorization.Operations;

namespace BugChang.DES.Core.Exchanges.ExchangeObjects
{
    public class ExchangeObjectOperations : IOperations
    {
        private const string Module = "ExchangeObject";
        public string GetMenuUrl()
        {
            return "/ExchangeObject/Index";
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

        public Operation AssignObjectSigner => new Operation
        {
            Name = "分配签收人",
            Code = $"{Module}.{nameof(AssignObjectSigner)}"
        };
    }
}
