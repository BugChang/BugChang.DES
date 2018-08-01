using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Application.HardWares;
using BugChang.DES.Application.HardWares.Dtos;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BugChang.DES.Web.Mvc.Controllers
{
    public class HardWareController : BaseController
    {
        private readonly IHardWareAppService _hardWareAppService;

        public HardWareController(IHardWareAppService hardWareAppService)
        {
            _hardWareAppService = hardWareAppService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GeSettings(string macAddress = "")
        {
            var hardWares = await _hardWareAppService.GetSettings(macAddress);
            return Json(hardWares);
        }

        [HttpPost]
        public async Task<IActionResult> Save(string postData)
        {
            var parameters = postData.Split('&');
            var macAddress = parameters.FirstOrDefault(a => a.Contains("macAddress"))?.Split('=')[1];
            var barcode6052 = parameters.FirstOrDefault(a => a.Contains("BarcodePrint6025"));
            var barcodePrint9030 = parameters.FirstOrDefault(a => a.Contains("BarcodePrint9030"));
            var barcodePrint80130 = parameters.FirstOrDefault(a => a.Contains("BarcodePrint80130"));
            var laserPrintA4 = parameters.FirstOrDefault(a => a.Contains("LaserPrintA4"));
            var laserPrintB5 = parameters.FirstOrDefault(a => a.Contains("LaserPrintB5"));
            var idReadCard = parameters.FirstOrDefault(a => a.Contains("IdReadCard"));
            var idReadCardBaudRate = parameters.FirstOrDefault(a => a.Contains("IdReadCardBaudRate"));
            var icReadCard = parameters.FirstOrDefault(a => a.Contains("IcReadCard"));
            var icReadCardBaudRate = parameters.FirstOrDefault(a => a.Contains("IcReadCardBaudRate"));
            var scanGun = parameters.FirstOrDefault(a => a.Contains("ScanGun"));
            var scanGunBaudRate = parameters.FirstOrDefault(a => a.Contains("ScanGunBaudRate"));
            var cpuReadCard = parameters.FirstOrDefault(a => a.Contains("CpuReadCard"));
            var cpuReadCardBaudRate = parameters.FirstOrDefault(a => a.Contains("CpuReadCardBaudRate"));
            var hardWares = new List<HardWareSaveDto>
            {
                new HardWareSaveDto
                {
                    DeviceCode = macAddress,
                    HardWareType = 0,
                    Value = barcode6052?.Split('=')[1]
                },
                new HardWareSaveDto
                {
                    DeviceCode = macAddress,
                    HardWareType = 1,
                    Value = barcodePrint9030?.Split('=')[1]
                },
                new HardWareSaveDto
                {
                    DeviceCode = macAddress,
                    HardWareType = 2,
                    Value = barcodePrint80130?.Split('=')[1]
                },
                new HardWareSaveDto
                {
                    DeviceCode = macAddress,
                    HardWareType = 3,
                    Value = laserPrintA4?.Split('=')[1]
                },
                new HardWareSaveDto
                {
                    DeviceCode = macAddress,
                    HardWareType =4,
                    Value = laserPrintB5?.Split('=')[1]
                },
                new HardWareSaveDto
                {
                    DeviceCode = macAddress,
                    HardWareType =5,
                    Value = idReadCard?.Split('=')[1],
                    BaudRate = idReadCardBaudRate?.Split('=')[1]
                },
                new HardWareSaveDto
                {
                    DeviceCode = macAddress,
                    HardWareType = 6,
                    Value = icReadCard?.Split('=')[1],
                    BaudRate = icReadCardBaudRate?.Split('=')[1]
                },
                new HardWareSaveDto
                {
                    DeviceCode = macAddress,
                    HardWareType = 7,
                    Value = scanGun?.Split('=')[1],
                    BaudRate = scanGunBaudRate?.Split('=')[1],
                },
                new HardWareSaveDto
                {
                    DeviceCode = macAddress,
                    HardWareType =8,
                    Value = cpuReadCard?.Split('=')[1],
                    BaudRate = cpuReadCardBaudRate?.Split('=')[1]
                }
            };
            var result = await _hardWareAppService.SaveHardWareSettings(hardWares, macAddress);
            return Json(result);
        }
    }
}
