namespace BugChang.DES.Core.Authorization.Operations
{
    public class RoleOperations : IOperations
    {
        public string GetMenuUrl()
        {
            return "/Role/Index";
        }

        public string GetModuleName()
        {
            return "Role";
        }

        public Operation RoleCreate => new Operation
        {
            Name = Operation.CreateName,
            Code = "Role.Create"
        };

        public Operation RoleEdit => new Operation
        {
            Name = Operation.EditName,
            Code = "Role.Edit"
        };

        public Operation RoleDelete => new Operation
        {
            Name = Operation.DeleteName,
            Code = "Role.Delete"
        };

        public Operation AssignmentsMenus => new Operation
        {
            Name = "分配菜单",
            Code = "Role.AssignmentsMenus"
        };

        public Operation AssignmentsOperations => new Operation
        {
            Name = "分配操作",
            Code = "Role.AssignmentsOperations"
        };

        public Operation DataPermissions => new Operation
        {
            Name = "数据权限",
            Code = "Role.DataPermissions"
        };

    }
}
