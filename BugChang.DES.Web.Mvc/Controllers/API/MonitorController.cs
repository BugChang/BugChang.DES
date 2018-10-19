using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Application.Boxs;
using BugChang.DES.Application.Boxs.Dtos;
using BugChang.DES.Application.Monitor.Dtos;
using BugChang.DES.Application.Monitors;
using BugChang.DES.Core.Monitor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BugChang.DES.Web.Mvc.Controllers.API
{
    [Route("Api/[controller]/[action]")]
    public class MonitorController : Controller
    {
        private readonly IMonitorAppService _monitorAppService;
        private readonly IBoxAppService _boxAppService;
        private readonly ILogger<MonitorController> _logger;
        public MonitorController(IMonitorAppService monitorAppService, IBoxAppService boxAppService, ILogger<MonitorController> logger)
        {
            _monitorAppService = monitorAppService;
            _boxAppService = boxAppService;
            _logger = logger;
        }

        /// <summary>
        /// 条码扫描
        /// </summary>
        /// <param name="barcodeNo">条码号</param>
        /// <param name="placeId">场所ID</param>
        /// <returns></returns>
        public async Task<IActionResult> CheckBarcodeType(string barcodeNo, int placeId)
        {
            try
            {
                var result = await _monitorAppService.CheckBarcodeType(barcodeNo, placeId);
                return Json(result);
            }
            catch (Exception e)
            {
                _logger.LogError("条码扫描错误：" + e);
                return Json(new CheckBarcodeModel());
            }

        }



        /// <summary>
        /// 获取所有箱格
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> GetBoxSetInfo(int placeId)
        {
            try
            {
                var boxs = await _monitorAppService.GetAllBoxs(placeId);
                return Json(boxs);
            }
            catch (Exception e)
            {
                _logger.LogError("获取箱格配置错误：" + e);
                return Json(new List<BoxListDto>());
            }

        }


        /// <summary>
        /// 获取所有箱组
        /// </summary>
        /// <param name="placeId"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetGroupSetInfo(int placeId)
        {
            try
            {
                //暂时不考虑B型箱
                var boxs = await _monitorAppService.GetAllBoxs(placeId);
                var boxGroups = boxs.Select(a => new BoxGroupListDto { Name = a.Name.ToString(), FrontCameraBn = a.FrontBn, FrontDigitalVein = a.FrontBn, FrontReadCardBn = a.FrontBn, FrontScanBn = a.FrontBn, FrontShowBn = a.FrontBn, FrontSoundBn = a.FrontBn, Boxs = a.Id.ToString() }).ToList();
                //var boxGroups = boxs.Select(a => new BoxGroupListDto { Name = a.Name.ToString(), FrontCameraBn = "BN00198", FrontDigitalVein = "BN00198", FrontReadCardBn = "BN00198", FrontScanBn = "BN00198", FrontShowBn = "BN00198", FrontSoundBn = "BN00198", Boxs = a.Id.ToString() }).ToList();
                return Json(boxGroups);
            }
            catch (Exception e)
            {
                _logger.LogError("获取箱组信息错误：" + e);
                return Json(new List<BoxGroupListDto>());
            }

        }

        /// <summary>
        /// 获取箱子基本信息
        /// </summary>
        /// <param name="boxId"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetBoxLetterCount(int boxId)
        {
            try
            {
                var box = await _monitorAppService.GetBox(boxId);
                return Json(box);
            }
            catch (Exception e)
            {
                _logger.LogError("获取箱格信件错误：" + e);
                return Json(new BoxListDto());
            }

        }

        /// <summary>
        /// 检查证卡类型
        /// </summary>
        /// <param name="cardValue"></param>
        /// <param name="placeId"></param>
        /// <param name="bn"></param>
        /// <returns></returns>
        public async Task<IActionResult> CheckCardType(string cardValue, int placeId, string bn)
        {
            try
            {
                var result = new CheckCardTypeModel();
                _logger.LogWarning($"placeId:{placeId};bn:{bn};cardValue:{cardValue}");
                var box = await _boxAppService.GetBoxByPlaceBn(bn, placeId);
                _logger.LogWarning($"box:{JsonConvert.SerializeObject(box)}");
                if (box != null)
                {
                    result = await _monitorAppService.CheckCardType(placeId, box.Id, cardValue);
                }
                return Json(result);
            }
            catch (Exception e)
            {
                _logger.LogError("检查证卡类型错误：" + e);
                return Json(new CheckCardTypeModel());
            }

        }

        public async Task<IActionResult> SaveLetter(int placeId, string barCode, int no, int fileCount, bool isJiaJi)
        {
            try
            {
                var result = await _monitorAppService.SaveLetter(placeId, barCode, no, 1, isJiaJi);
                return Json(result);
            }
            catch (Exception e)
            {
                _logger.LogError("保存信件信息错误：" + e);
                return Json(0);
            }

        }

        public async Task<IActionResult> Box_UserGetLetter(int no, string cardValue, int placeId)
        {
            try
            {
                _logger.LogWarning($"no：{no},cardValue:{cardValue},placeId:{placeId}");
                var result = await _monitorAppService.UserGetLetter(no, cardValue, placeId);
                return Json(result > 0);

            }
            catch (Exception e)
            {
                _logger.LogError("用户取件错误：" + e);
                return Json(false);
            }


        }
    }
}
