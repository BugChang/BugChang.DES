using BugChang.DES.Core.Authorization.Operations;

namespace BugChang.DES.Core.Authorization.Menus
{
    public class MenuOperations : IOperations
    {
        public string GetMenuUrl()
        {
            return "/Menu/Index";
        }

        public string GetModuleName()
        {
            return "Menu";
        }

        public Operation MenuCreate => new Operation
        {
            Name = Operation.CreateName,
            Code = "Menu.Create"
        };

        public Operation MenuEdit => new Operation
        {
            Name = Operation.EditName,
            Code = "Menu.Edit"
        };

        public Operation MenuDelete => new Operation
        {
            Name = Operation.DeleteName,
            Code = "Menu.Delete"
        };
    }
}
