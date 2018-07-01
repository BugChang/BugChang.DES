using System;
using System.Collections.Generic;
using System.Security.Claims;
using BugChang.DES.Application.Roles;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BugChang.DES.Web.Mvc.Filters
{
    public class OperationFilter : Attribute, IActionFilter
    {
        private readonly IRoleAppService _roleAppService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly string _operationCode;
        public OperationFilter(IRoleAppService roleAppService, string operationCode, IHostingEnvironment hostingEnvironment)
        {
            _roleAppService = roleAppService;
            _operationCode = operationCode;
            _hostingEnvironment = hostingEnvironment;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!_hostingEnvironment.IsDevelopment())
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
                    context.HttpContext.ForbidAsync();
                    context.Result = new ContentResult()
                    {
                        Content = "权限不足！"
                    };
                }
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {

        }
    }
}
