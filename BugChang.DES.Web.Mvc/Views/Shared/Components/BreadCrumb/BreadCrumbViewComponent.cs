using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace BugChang.DES.Web.Mvc.Views.Shared.Components.BreadCrumb
{
    public class BreadCrumbViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string activeMenuName = "")
        {
            activeMenuName = activeMenuName ?? "";
            var model = activeMenuName.Split("-").ToList();
            return View(model);
        }
    }
}
