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

        /// <summary>
        /// 检查证卡类型
        /// </summary>
        /// <param name="cardValue"></param>
        /// <param name="placeId"></param>
        /// <param name="boxId"></param>
        /// <returns></returns>
        public async Task<IActionResult> CheckCardType(string cardValue, int placeId, int boxId)
        {
            var result = await _monitorAppService.CheckCardType(placeId, boxId, cardValue);
            return Json(result);
        }

        public async Task<IActionResult> SaveLetter(int pacleId, string barCode, int boxId, int fileCount, bool isJiaJi)
        {
            var result = await _monitorAppService.SaveLetter(pacleId, barCode, boxId, 1, isJiaJi);
            return Json(result);
        }

        public async Task<IActionResult> Box_UserGetLetter(int boxId, string cardValue, int placeId)
        {
            var result = await _monitorAppService.UserGetLetter(boxId, cardValue, placeId);
            return Json(result);
        
        }
    }
}
