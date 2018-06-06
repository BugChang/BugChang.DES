using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Application.Menus;
using Microsoft.AspNetCore.Mvc;

namespace BugChang.DES.Web.Mvc.Views.Shared.Components.BreadCrumb
{
    public class BreadCrumbViewComponent : ViewComponent
    {
        private readonly IMenuAppService _menuAppService;

        public BreadCrumbViewComponent(IMenuAppService menuAppService)
        {
            _menuAppService = menuAppService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string url)
        {
            var breadCrumb = await _menuAppService.GetMenuBreadCrumbAsync(url);
            if (!string.IsNullOrWhiteSpace(breadCrumb))
            {
                var model = breadCrumb.Split('#').ToList();
                return View(model);
            }

            return View(new List<string>());
        }
    }
}
