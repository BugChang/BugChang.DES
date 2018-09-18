using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Application.BackUps;
using BugChang.DES.Core.Commons;
using BugChang.DES.Web.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BugChang.DES.Web.Mvc.Controllers
{
    [ServiceFilter(typeof(RefererFilter))]
    public class BackUpController : BaseController
    {

        private readonly IBackUpAppService _backUpAppService;

        public BackUpController(IBackUpAppService backUpAppService)
        {
            _backUpAppService = backUpAppService;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> BackUpNow(string remark)
        {
            var result = await _backUpAppService.BackUpNow(2, CurrentUser.DisplayName, remark);
            return Json(result);
        }

        public async Task<IActionResult> GetBackUps(int draw, int start, int length)
        {
            var keywords = Request.Query["search[value]"];
            var pageSearchDto = new PageSearchCommonModel
            {
                Keywords = keywords,
                Take = length,
                Skip = start
            };
            var pagereslut = await _backUpAppService.GetPagingAysnc(pageSearchDto);
            var json = new
            {
                draw,
                recordsTotal = pagereslut.Total,
                recordsFiltered = pagereslut.Total,
                data = pagereslut.Rows
            };
            return Json(json);
        }
    }
}
