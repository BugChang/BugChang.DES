using System;
using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Application.Menus;
using Microsoft.AspNetCore.Mvc;

namespace BugChang.DES.Web.Mvc.Views.Shared.Components.SideBarNav
{
    public class SideBarNavViewComponent : ViewComponent
    {
        private readonly IMenuAppService _menuAppService;

        public SideBarNavViewComponent(IMenuAppService menuAppService)
        {
            _menuAppService = menuAppService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string activeMenuName = "")
        {
            var userId = HttpContext.User.Claims.Single(a => a.Type == "Id").Value;
            var model = new SideBarNavViewModel
            {
                Menus = await _menuAppService.GetUserMenusAsync(Convert.ToInt32(userId)),
                ActiveMenuName = activeMenuName ?? ""
            };
            return View(model);
        }
    }
}
