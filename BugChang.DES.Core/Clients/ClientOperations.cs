using System;
using System.Collections.Generic;
using System.Text;
using BugChang.DES.Core.Authorization.Operations;

namespace BugChang.DES.Core.Clients
{
   public class ClientOperations:IOperations
    {
        private const string Module = "Client";
        public string GetMenuUrl()
        {
            return "/Client/Index";
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
    }
}
