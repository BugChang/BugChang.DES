namespace BugChang.DES.Core.Authorization.Operations
{
    public class DepartmentOperations : IOperations
    {
        public const string Module = "Department";


        public Operation DepartmentCreate => new Operation
        {
            Name = Operation.CreateName,
            Code = "Department.Create"
        };

        public Operation DepartmentEdit => new Operation
        {
            Name = Operation.EditName,
            Code = "Department.Edit"
        };
        public Operation DepartmentDelete => new Operation
        {
            Name = Operation.DeleteName,
            Code = "Department.Delete"
        };

        public string GetMenuUrl()
        {
            return "/Department/Index";
        }

        public string GetModuleName()
        {
            return "Department";
        }
    }
}
