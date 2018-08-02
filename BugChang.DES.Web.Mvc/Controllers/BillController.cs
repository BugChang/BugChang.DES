using System.Threading.Tasks;
using BugChang.DES.Application.Clients;
using Microsoft.AspNetCore.Mvc;

namespace BugChang.DES.Web.Mvc.Controllers
{
    public class BillController : BaseController
    {
        private readonly IClientAppService _clientAppService;

        public BillController(IClientAppService clientAppService)
        {
            _clientAppService = clientAppService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult List()
        {
            return View();
        }

        public async Task<IActionResult> CheckReceive(string deviceCode)
        {
            var client = await _clientAppService.GetClient(deviceCode);

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
