using BugChang.DES.Core.Authorization.Operations;

namespace BugChang.DES.Core.Authentication
{
    public class CardOperations : IOperations
    {
        public const string Module = "Card";

        public Operation CardCreate => new Operation
        {
            Name = Operation.CreateName,
            Code = "Card.Create"
        };

        public Operation CardEdit => new Operation
        {
            Name = Operation.EditName,
            Code = "Card.Edit"
        };
        public Operation CardDelete => new Operation
        {
            Name = Operation.DeleteName,
            Code = "Card.Delete"
        };

        public Operation CardEnabled => new Operation
        {
            Name = "证卡启用",
            Code = "Card.Enabled"
        };

        public string GetMenuUrl()
        {
            return "/Card/Index";
        }

        public string GetModuleName()
        {
            return "Card";
        }
    }
}
