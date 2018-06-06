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

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var url = Request.Path;
            var model = await _menuAppService.GetMenuBreadCrumbAsync(url);
            return View(model);
        }
    }
}
