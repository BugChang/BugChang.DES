using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using BugChang.DES.Application.Roles;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Razor.Extensions;

namespace BugChang.DES.Web.Mvc.Filters
{
    public class OperationFilter : Attribute, IActionFilter
    {
        private readonly IRoleAppService _roleAppService;
        private readonly string _operationCode;
        public OperationFilter(IRoleAppService roleAppService, string operationCode)
        {
            _roleAppService = roleAppService;
            _operationCode = operationCode;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var lstRoleId = new List<int>();
            var roleClaims = context.HttpContext.User.FindAll("RoleId");
            foreach (Claim roleClaim in roleClaims)
            {
                lstRoleId.Add(Convert.ToInt32(roleClaim.Value));
            }

            var userOperationCodes = _roleAppService.GetRoleOperationCodes(String.Empty, lstRoleId);
            if (!userOperationCodes.Contains(_operationCode))
            {
                context.Result = new ForbidResult();
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {

        }
    }
}
