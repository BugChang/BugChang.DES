using System.Threading.Tasks;
using BugChang.DES.Application.Monitor;
using Microsoft.AspNetCore.Mvc;

namespace BugChang.DES.Web.Mvc.Controllers.API
{
    [Route("Api/[controller]/[action]")]
    public class MonitorController : Controller
    {
        private readonly IMonitorAppService _monitorAppService;

        public MonitorController(IMonitorAppService monitorAppService)
        {
            _monitorAppService = monitorAppService;
        }

        /// <summary>
        /// 条码扫描
        /// </summary>
        /// <param name="barcodeNo">条码号</param>
        /// <param name="placeId">场所ID</param>
        /// <returns></returns>
        public async Task<IActionResult> CheckBarcodeType(string barcodeNo, int placeId)
        {
            var result = await _monitorAppService.CheckBarcodeType(barcodeNo, placeId);
            return Json(result);
        }
    }
}
