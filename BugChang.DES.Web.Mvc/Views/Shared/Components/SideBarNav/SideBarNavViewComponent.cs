using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BugChang.DES.Web.Mvc.Views.Shared.Components.SideBarNav
{
    public class SideBarNavViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
    }
}
