using System.Collections.Generic;
using BugChang.DES.Core.Authorization.Operations;

namespace BugChang.DES.Application.Operations
{
   public class OperationAppService:IOperationAppService
   {
       private readonly OperationManager _operationManager;

       public OperationAppService(OperationManager operationManager)
       {
           _operationManager = operationManager;
       }

       public IList<Operation> GetOperationsByUrl(string url)
       {
           return _operationManager.GetOperationsByUrl(url);
       }
    }
}
