using System;
using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Application.Departments;
using BugChang.DES.Application.Groups;
using BugChang.DES.Application.Letters;
using BugChang.DES.Application.Letters.Dtos;
using BugChang.DES.Core.Commons;
using BugChang.DES.Core.Letters;
using BugChang.DES.Web.Mvc.Filters;
using BugChang.DES.Web.Mvc.Models.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace BugChang.DES.Web.Mvc.Controllers
{
    public class LetterController : BaseController
    {
        private readonly ILetterAppService _letterAppService;
        private readonly IDepartmentAppService _departmentAppService;
        private readonly IGroupAppService _groupAppService;
        public LetterController(ILetterAppService letterAppService, IDepartmentAppService departmentAppService, IGroupAppService groupAppService)
        {
            _letterAppService = letterAppService;
            _departmentAppService = departmentAppService;
            _groupAppService = groupAppService;
        }

        [TypeFilter(typeof(MenuFilter))]
        public IActionResult Receive()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Receive(ReceiveLetterEditDto receiveLetter)
        {
            var result = new ResultEntity();
            if (ModelState.IsValid)
            {
                receiveLetter.SetCreateOrUpdateInfo(CurrentUser.UserId);
                receiveLetter.LetterType = EnumLetterType.收信;
                receiveLetter.SendDepartmentId = CurrentUser.DepartmentId;
                result = await _letterAppService.AddReceiveLetter(receiveLetter);
            }
            else
            {
                result.Message = ModelState.Values
                    .FirstOrDefault(a => a.ValidationState == ModelValidationState.Invalid)?.Errors.FirstOrDefault()
                    ?.ErrorMessage;
            }
            return Json(result);
        }

        public async Task<IActionResult> GetTodayReceiveLetters(int draw, int start, int length)
        {
            var keywords = Request.Query["search[value]"];
            var pageSearchDto = new PageSearchCommonModel
            {
                Keywords = keywords,
                Take = length,
                Skip = start
            };
            var pagereslut = await _letterAppService.GetTodayReceiveLetters(pageSearchDto);
            var json = new
            {
                draw,
                recordsTotal = pagereslut.Total,
                recordsFiltered = pagereslut.Total,
                data = pagereslut.Rows
            };
            return Json(json);
        }

        public async Task<IActionResult> GetReceiveBarcode(int id)
        {
            var letter = await _letterAppService.GetReceiveBarcode(id);
            return Json(letter);
        }

        public IActionResult Send()
        {
            return View();
        }

        public async Task<IActionResult> GetReceiveGroupSelect()
        {
            var groups = await _groupAppService.GetReceiveLetterGroups();
            var json = groups.Select(a => new SimpleTreeViewModel
            {
                Id = a.Id,
                Name = a.Name
            });
            return Json(json);
        }

        public async Task<IActionResult> GetReceiveDetailSelect(int id)
        {
            var departments = await _departmentAppService.GetAllAsync();
            var groupDetails = await _groupAppService.GetGroupDetails(id);
            departments = departments.Where(a => groupDetails.Select(b => b.DepartmentId).Contains(a.Id)).ToList();
            var json = departments.Select(a => new SimpleTreeViewModel
            {
                Id = a.Id,
                ParentId = a.ParentId,
                Name = a.Name
            });
            return Json(json);
        }

        public IActionResult ReceiveList()
        {
            return View();
        }

        public async Task<IActionResult> GetReceiveLetters(int draw, int start, int length)
        {
            var pageSearchDto = new ReceivePageSerchModel
            {
                Take = length,
                Skip = start
            };
            pageSearchDto.SetTimeValue(Request.Query["beginTime"], Request.Query["endTime"]);
            pageSearchDto.SendDepartmentId = Convert.ToInt32(Request.Query["sendDepartmentId"]);
            pageSearchDto.ReceiveDepartmentId = CurrentUser.DepartmentId;
            pageSearchDto.ShiJiNo = Request.Query["shiJiNo"];
            pageSearchDto.LetterNo = Request.Query["letterNo"];
            var pagereslut = await _letterAppService.GetReceiveLetters(pageSearchDto);
            var json = new
            {
                draw,
                recordsTotal = pagereslut.Total,
                recordsFiltered = pagereslut.Total,
                data = pagereslut.Rows
            };
            return Json(json);
        }

        public async Task<IActionResult> GetDepartments()
        {
            var departments = await _departmentAppService.GetAllAsync();
            var json = departments.Select(a => new SelectViewModel
            {
                Id = a.Id,
                Text = a.FullName
            }).ToList();
            json.Insert(0,new SelectViewModel
            {
                Id = 0,
                Text = "请选择"
            });
            return Json(json);
        }
    }
}
