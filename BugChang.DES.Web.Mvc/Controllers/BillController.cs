using Microsoft.AspNetCore.Mvc;

namespace BugChang.DES.Web.Mvc.Controllers
{
    public class BillController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult List()
        {
            return View();
        }

        public IActionResult CheckReceive(string deviceCode)
        {
            //根据deviceCode找到当前是哪个场所
            return Json(null);
        }

        public IActionResult CheckSend(string deviceCode)
        {
            return Json(null);
        }

        public IActionResult CheckSendAndReceive(string deviceCode)
        {
            return Json(null);
        }

    }
}
