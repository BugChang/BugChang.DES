using Microsoft.AspNetCore.Mvc;

namespace BugChang.DES.Web.Mvc.Controllers
{
    public class LogController : BaseController
    {
        // GET: /<controller>/
        public IActionResult Audit()
        {
            return View();
        }

        public IActionResult System()
        {
            return View();
        }
    }
}
