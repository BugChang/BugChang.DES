using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BugChang.DES.Application.Menus;
using BugChang.DES.Application.Menus.Dtos;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace BugChang.DES.Web.Mvc.Views.Shared.Components.SideBarNav
{
    public class SideBarNavViewComponent : ViewComponent
    {
        private readonly IMenuAppService _menuAppService;
        private readonly IHostingEnvironment _environment;

        public SideBarNavViewComponent(IMenuAppService menuAppService, IHostingEnvironment environment)
        {
            _menuAppService = menuAppService;
            _environment = environment;
        }

        public async Task<IViewComponentResult> InvokeAsync(string url = "")
        {
            IList<MenuListDto> menus;
            if (_environment.IsDevelopment())
            {
                menus = await _menuAppService.GetAllRootAsync();
            }
            else
            {
                var roles = HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Role).Select(a => a.Value).ToList();
                menus = await _menuAppService.GetUserMenusAsync(roles);
            }
            var breadCrumb = await _menuAppService.GetMenuBreadCrumbAsync(url);
            var model = new SideBarNavViewModel
            {
                Menus = menus,
                ActiveMenuName = breadCrumb ?? ""
            };
            return View(model);
        }
    }
}
