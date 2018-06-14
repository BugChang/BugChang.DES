using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BugChang.DES.Core.Authorization.Operations
{
    public class OperationManager
    {
        public IList<Operation> GetOperationsByUrl(string url)
        {
            IOperations departmentOperations=new DepartmentOperations();

            var operations = new List<Operation>();
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IOperations))));
            foreach (var v in types)
            {
                if (v.IsClass)
                {
                    (Activator.CreateInstance(v) as IOperations)?.GetMenuUrl();
                }
                var propertyList = v.GetProperties();

                foreach (var item in propertyList)
                {
                    var c = item.GetValue(departmentOperations, null);
                }
            }
            return operations;
        }
    }
}
