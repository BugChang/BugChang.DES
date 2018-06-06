using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Application.Menus;
using BugChang.DES.Web.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BugChang.DES.Web.Mvc.Filters
{
    public class BreadCrumbFilter:Attribute,IAsyncActionFilter
    {
        private readonly IMenuAppService _menuAppService;

        public BreadCrumbFilter(IMenuAppService menuAppService)
        {
            _menuAppService = menuAppService;
        }

        public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var controller = context.RouteData.Values["Controller"].ToString();
            var action = context.RouteData.Values["Action"].ToString();
            var url = "/" + controller + "/" + action;
            var breadCrumb = _menuAppService.GetMenuBreadCrumbAsync(url);
            
            return Task.CompletedTask;
        }
    }
}
