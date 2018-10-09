using System;
using BugChang.DES.Application.DashBoards;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using BugChang.DES.Core.Tools;
using BugChang.DES.Web.Mvc.Filters;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BugChang.DES.Web.Mvc.Controllers
{
    [ServiceFilter(typeof(RefererFilter))]
    public class HomeController : BaseController
    {
        private readonly IDashBoardAppService _dashBoardAppService;



        public HomeController(IDashBoardAppService dashBoardAppService, IHostingEnvironment environment)
        {
            _dashBoardAppService = dashBoardAppService;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult HardDIskWarning()
        {
            ViewBag.HardDiskSpace = ComputerInfoHelper.GetHardDiskSpace("D");
            ViewBag.HardDiskUseageSpace = ComputerInfoHelper.GetHardDiskUseSpace("D");
            ViewBag.HardDiskUsageRate = ComputerInfoHelper.GetHardDiskUsageRate("D");
            ViewBag.CpuUsageRate = ComputerInfoHelper.GetCpuUsageRate();
            ViewBag.MemoryUsageRate = ComputerInfoHelper.GetMemoryUsageRate();
            return PartialView("_ComputerInfoComponent");
        }
    }

}
