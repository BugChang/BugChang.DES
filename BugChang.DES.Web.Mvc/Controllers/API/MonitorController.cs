using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Application.Monitor.Dtos;
using BugChang.DES.Application.Monitors;
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



        /// <summary>
        /// 获取所有箱格
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> GetBoxSetInfo(int placeId)
        {
            var boxs = await _monitorAppService.GetAllBoxs(placeId);
            return Json(boxs);
        }


        /// <summary>
        /// 获取所有箱组
        /// </summary>
        /// <param name="placeId"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetGroupSetInfo(int placeId)
        {
            //暂时不考虑B型箱
            var boxs = await _monitorAppService.GetAllBoxs(placeId);
            var boxGroups = boxs.Select(a => new BoxGroupListDto { Name = a.Id.ToString(), FrontCameraBn = a.FrontBn, FrontDigitalVein = a.FrontBn, FrontReadCardBn = a.FrontBn, FrontScanBn = a.FrontBn, FrontShowBn = a.FrontBn, FrontSoundBn = a.FrontBn, Boxs = a.Id.ToString() }).ToList();
            return Json(boxGroups);
        }

        /// <summary>
        /// 获取箱子基本信息
        /// </summary>
        /// <param name="boxId"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetBoxLetterCount(int boxId)
        {
            var box = await _monitorAppService.GetBox(boxId);
            return Json(box);
        }

        public async Task<IActionResult> CheckCardType(string cardValue, int placeId, int boxId)
        {
            //todo:如果是本场所管理员，负责的流转对象的箱子为签收，不负责的为开门
            //todo:如果不是本场所管理员，只负责签收负责的流转对象
            return Json(null);
        }
    }
}
