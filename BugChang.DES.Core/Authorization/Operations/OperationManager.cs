using System;
using System.Collections.Generic;
using System.Linq;
using BugChang.DES.Core.Authorization.Roles;

namespace BugChang.DES.Core.Authorization.Operations
{
    public class OperationManager
    {
        /// <summary>
        /// 通过URL获取操作列表
        /// </summary>
        /// <param name="url">URL</param>
        /// <param name="module">URL对应模块</param>
        /// <returns></returns>
        public IList<Operation> GetOperationsByUrl(string url, out string module)
        {
            module = string.Empty;
            var operations = new List<Operation>();
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IOperations))));
            foreach (var v in types)
            {
                var obj = Activator.CreateInstance(v);
                if (v.IsClass)
                {
                    var operation = obj as IOperations;
                    if (operation?.GetMenuUrl() == url)
                    {
                        if (operation != null)
                            module = operation.GetModuleName();
                        var propertyList = v.GetProperties();
                        operations.AddRange(propertyList.Select(item => item.GetValue(obj, null) as Operation));
                        break;
                    }
                }
            }
            return operations;
        }
    }
}
