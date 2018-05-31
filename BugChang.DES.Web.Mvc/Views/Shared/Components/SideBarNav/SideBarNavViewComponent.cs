using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<IViewComponentResult> InvokeAsync(string activeMenuName = "")
        {
            var userId = HttpContext.User.Claims.Single(a => a.Type == "Id").Value;
            IList<MenuDto> menus;
            if (_environment.IsDevelopment())
            {
                menus = await _menuAppService.GetAllRootAsync();
            }
            else
            {
                menus = await _menuAppService.GetUserMenusAsync(Convert.ToInt32(userId));
            }
            var model = new SideBarNavViewModel
            {
                Menus = menus,
                ActiveMenuName = activeMenuName ?? ""
            };
            return View(model);
        }
    }
}
