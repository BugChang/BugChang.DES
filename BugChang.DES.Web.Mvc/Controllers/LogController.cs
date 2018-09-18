using System.Threading.Tasks;
using BugChang.DES.Application.Logs;
using BugChang.DES.Core.Commons;
using BugChang.DES.Web.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace BugChang.DES.Web.Mvc.Controllers
{
    [ServiceFilter(typeof(RefererFilter))]
    public class LogController : BaseController
    {

        private readonly ILogAppService _logAppService;

        public LogController(ILogAppService logAppService)
        {
            _logAppService = logAppService;
        }

        public IActionResult Audit()
        {
            return View();
        }

        public async Task<IActionResult> GetSystemLogs(int draw, int start, int length)
        {
            var keywords = Request.Query["search[value]"];
            var pageSearchDto = new PageSearchCommonModel
            {
                Keywords = keywords,
                Take = length,
                Skip = start
            };
            var pagereslut = await _logAppService.GetSystemLogs(pageSearchDto);
            var json = new
            {
                draw,
                recordsTotal = pagereslut.Total,
                recordsFiltered = pagereslut.Total,
                data = pagereslut.Rows
            };
            return Json(json);
        }

        public async Task<IActionResult> GetAuditLogs(int draw, int start, int length)
        {
            var keywords = Request.Query["search[value]"];
            var pageSearchDto = new PageSearchCommonModel
            {
                Keywords = keywords,
                Take = length,
                Skip = start
            };
            var pagereslut = await _logAppService.GetAuditLogs(pageSearchDto);
            var json = new
            {
                draw,
                recordsTotal = pagereslut.Total,
                recordsFiltered = pagereslut.Total,
                data = pagereslut.Rows
            };
            return Json(json);
        }

        public IActionResult System()
        {
            return View();
        }
    }
}
