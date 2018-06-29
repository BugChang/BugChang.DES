using BugChang.DES.Core.Authorization.Operations;

namespace BugChang.DES.Core.Exchanges.Boxs
{
    public class BoxOperations : IOperations
    {
        public string GetMenuUrl()
        {
            return "/Box/Index";
        }

        public string GetModuleName()
        {
            return "Box";
        }
    }
}
