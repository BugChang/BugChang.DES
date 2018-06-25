using BugChang.DES.Core.Authorization.Operations;

namespace BugChang.DES.Core.Authorization.Users
{
    public class UserOperations : IOperations
    {
        public string GetMenuUrl()
        {
            return "/User/Index";
        }

        public string GetModuleName()
        {
            return "User";
        }

        public Operation UserCreate => new Operation
        {
            Name = Operation.CreateName,
            Code = "User.Create"
        };

        public Operation UserEdit => new Operation
        {
            Name = Operation.EditName,
            Code = "User.Edit"
        };

        public Operation UserDelete => new Operation
        {
            Name = Operation.DeleteName,
            Code = "User.Delete"
        };

        public Operation AssigningRoles => new Operation
        {
            Name = "分配角色",
            Code = "User.AssigningRoles"
        };


    }
}
