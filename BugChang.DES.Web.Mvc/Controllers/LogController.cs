using System;
using System.IO;
using System.Threading.Tasks;
using BugChang.DES.Application.Logs;
using BugChang.DES.Core.Commons;
using BugChang.DES.Core.Logs;
using BugChang.DES.Web.Mvc.Filters;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace BugChang.DES.Web.Mvc.Controllers
{
    [ServiceFilter(typeof(RefererFilter))]
    public class LogController : BaseController
    {

        private readonly ILogAppService _logAppService;
        private readonly IHostingEnvironment _hostingEnvironment;

        public LogController(ILogAppService logAppService, IHostingEnvironment hostingEnvironment)
        {
            _logAppService = logAppService;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Audit()
        {
            return View();
        }

        public async Task<IActionResult> GetSystemLogs(int draw, int start, int length)
        {
            var pageSearchDto = new LogPageSerchModel
            {
                Take = length,
                Skip = start
            };
            pageSearchDto.SetTimeValue(Request.Query["BeginTime"], Request.Query["EndTime"]);
            pageSearchDto.Level = Convert.ToInt32((Request.Query["Level"]));
            pageSearchDto.Title = Request.Query["Title"].ToString();
            pageSearchDto.Content = Request.Query["Content"];
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
            var pageSearchDto = new LogPageSerchModel()
            {
                Take = length,
                Skip = start
            };
            pageSearchDto.SetTimeValue(Request.Query["BeginTime"], Request.Query["EndTime"]);
            pageSearchDto.Level = Convert.ToInt32((Request.Query["Level"]));
            pageSearchDto.Title = Request.Query["Title"].ToString();
            pageSearchDto.Content = Request.Query["Content"];
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

        public async Task<IActionResult> ExportSystenLog()
        {
            var pageSearchDto = new LogPageSerchModel()
            {
                Take = 10000,
                Skip = 0
            };
            pageSearchDto.SetTimeValue(Request.Form["BeginTime"], Request.Form["EndTime"]);
            pageSearchDto.Level = Convert.ToInt32((Request.Form["Level"]));
            pageSearchDto.Title = Request.Form["Title"].ToString();
            pageSearchDto.Content = Request.Form["Content"];
            var pagereslut = await _logAppService.GetSystemLogs(pageSearchDto);
            string sWebRootFolder = _hostingEnvironment.WebRootPath;
            string sFileName = $"{Guid.NewGuid()}.xlsx";
            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            using (ExcelPackage package = new ExcelPackage(file))
            {
                // 添加worksheet
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("aspnetcore");
                //添加头
                worksheet.Cells[1, 1].Value = "主键";
                worksheet.Cells[1, 2].Value = "级别";
                worksheet.Cells[1, 3].Value = "标题";
                worksheet.Cells[1, 4].Value = "内容";
                worksheet.Cells[1, 5].Value = "时间";
                worksheet.Cells[1, 6].Value = "数据";
                //添加值
                int i = 2;
                foreach (var row in pagereslut.Rows)
                {
                    worksheet.Cells["A" + i].Value = row.Id;
                    worksheet.Cells["B" + i].Value = row.Level;
                    worksheet.Cells["C" + i].Value = row.Title;
                    worksheet.Cells["D" + i].Value = row.Content;
                    worksheet.Cells["E" + i].Value = row.CreateTime;
                    worksheet.Cells["F" + i].Value = row.Data;
                    i++;
                }

                package.Save();
            }
            return File(sFileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public async Task<IActionResult> ExportAuditLog()
        {
            var pageSearchDto = new LogPageSerchModel()
            {
                Take = 10000,
                Skip = 0
            };
            pageSearchDto.SetTimeValue(Request.Form["BeginTime"], Request.Form["EndTime"]);
            pageSearchDto.Level = Convert.ToInt32((Request.Form["Level"]));
            pageSearchDto.Title = Request.Form["Title"].ToString();
            pageSearchDto.Content = Request.Form["Content"];
            var pagereslut = await _logAppService.GetAuditLogs(pageSearchDto);
            string sWebRootFolder = _hostingEnvironment.WebRootPath;
            string sFileName = $"{Guid.NewGuid()}.xlsx";
            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            using (ExcelPackage package = new ExcelPackage(file))
            {
                // 添加worksheet
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("aspnetcore");
                //添加头
                worksheet.Cells[1, 1].Value = "主键";
                worksheet.Cells[1, 2].Value = "级别";
                worksheet.Cells[1, 3].Value = "标题";
                worksheet.Cells[1, 4].Value = "内容";
                worksheet.Cells[1, 5].Value = "时间";
                worksheet.Cells[1, 6].Value = "操作人";
                worksheet.Cells[1, 7].Value = "数据";
                //添加值
                int i = 2;
                foreach (var row in pagereslut.Rows)
                {
                    worksheet.Cells["A" + i].Value = row.Id;
                    worksheet.Cells["B" + i].Value = row.Level;
                    worksheet.Cells["C" + i].Value = row.Title;
                    worksheet.Cells["D" + i].Value = row.Content;
                    worksheet.Cells["E" + i].Value = row.CreateTime;
                    worksheet.Cells["F" + i].Value = row.OperatorName;
                    worksheet.Cells["G" + i].Value = row.Data;
                    i++;
                }

                package.Save();
            }
            return File(sFileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }
    }
}
