using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BugChang.DES.Application.Menus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BugChang.DES.Web.Mvc.Filters
{
    public class MenuFilter : Attribute, IAsyncAuthorizationFilter
    {
        private readonly IMenuAppService _menuAppService;

        public MenuFilter(IMenuAppService menuAppService)
        {
            _menuAppService = menuAppService;
        }
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.User.IsInRole("系统管理员"))
            {
                var roles = context.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Role).Select(a => a.Value).ToList();
                var controller = context.RouteData.Values["Controller"].ToString();
                var action = context.RouteData.Values["Action"].ToString();
                var url = "/" + controller + "/" + action;
                if (roles.Count == 0)
                {
                    context.Result = new ForbidResult();
                }
                else
                {
                    var hasMenu = await _menuAppService.HasMenu(roles, url);
                    if (!hasMenu)
                    {
                        context.Result = new ForbidResult();
                    }
                }
            }
        }
    }
}
