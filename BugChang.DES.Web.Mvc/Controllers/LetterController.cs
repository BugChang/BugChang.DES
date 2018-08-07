using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Application.Departments;
using BugChang.DES.Application.Groups;
using BugChang.DES.Application.Letters;
using BugChang.DES.Application.Letters.Dtos;
using BugChang.DES.Application.Users;
using BugChang.DES.Core.Commons;
using BugChang.DES.Core.Letters;
using BugChang.DES.Web.Mvc.Filters;
using BugChang.DES.Web.Mvc.Models.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BugChang.DES.Web.Mvc.Controllers
{
    public class LetterController : BaseController
    {
        private readonly ILetterAppService _letterAppService;
        private readonly IDepartmentAppService _departmentAppService;
        private readonly IGroupAppService _groupAppService;
        private readonly IUserAppService _userAppService;
        public LetterController(ILetterAppService letterAppService, IDepartmentAppService departmentAppService, IGroupAppService groupAppService, IUserAppService userAppService)
        {
            _letterAppService = letterAppService;
            _departmentAppService = departmentAppService;
            _groupAppService = groupAppService;
            _userAppService = userAppService;
        }

        #region 收信

        [TypeFilter(typeof(MenuFilter))]
        public IActionResult Receive()
        {
            return View();
        }

        public IActionResult ReceiveList()
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

        public async Task<IActionResult> GetReceiveLetters(int draw, int start, int length)
        {
            var pageSearchDto = new LetterPageSerchModel
            {
                Take = length,
                Skip = start
            };
            pageSearchDto.SetTimeValue(Request.Query["beginTime"], Request.Query["endTime"]);
            if (!string.IsNullOrWhiteSpace(Request.Query["sendDepartmentId"]))
            {
                pageSearchDto.SendDepartmentId = Convert.ToInt32(Request.Query["sendDepartmentId"]);
            }

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

        #endregion

        #region 发信

        public IActionResult Send()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Send(LetterSendEditDto sendLetter)
        {
            var result = new ResultEntity();
            if (ModelState.IsValid)
            {
                sendLetter.SetCreateOrUpdateInfo(CurrentUser.UserId);
                sendLetter.SendDepartmentId = CurrentUser.DepartmentId;
                result = await _letterAppService.AddSendLetter(sendLetter);
            }
            else
            {
                result.Message = ModelState.Values
                    .FirstOrDefault(a => a.ValidationState == ModelValidationState.Invalid)?.Errors.FirstOrDefault()
                    ?.ErrorMessage;
            }
            return Json(result);
        }

        public IActionResult SendList()
        {
            return View();
        }

        public async Task<IActionResult> GetSendLetters(int draw, int start, int length)
        {
            var pageSearchDto = new LetterPageSerchModel
            {
                Take = length,
                Skip = start
            };
            pageSearchDto.SetTimeValue(Request.Query["beginTime"], Request.Query["endTime"]);
            pageSearchDto.SendDepartmentId = CurrentUser.DepartmentId;
            if (!string.IsNullOrWhiteSpace(Request.Query["receiveDepartmentId"]))
            {
                pageSearchDto.ReceiveDepartmentId = Convert.ToInt32(Request.Query["receiveDepartmentId"]);
            }
            pageSearchDto.LetterNo = Request.Query["letterNo"];
            var pagereslut = await _letterAppService.GetSendLetters(pageSearchDto);
            var json = new
            {
                draw,
                recordsTotal = pagereslut.Total,
                recordsFiltered = pagereslut.Total,
                data = pagereslut.Rows
            };
            return Json(json);
        }

        public async Task<IActionResult> GetSendGroupSelect()
        {
            var groups = await _groupAppService.GetSendLetterGroups();
            var json = groups.Select(a => new SimpleTreeViewModel
            {
                Id = a.Id,
                Name = a.Name
            });
            return Json(json);
        }

        public async Task<IActionResult> GetTodaySendLetters(int draw, int start, int length)
        {
            var keywords = Request.Query["search[value]"];
            var pageSearchDto = new PageSearchCommonModel
            {
                DepartmentId = CurrentUser.DepartmentId,
                Keywords = keywords,
                Take = length,
                Skip = start
            };
            var pagereslut = await _letterAppService.GetTodaySendLetters(pageSearchDto);
            var json = new
            {
                draw,
                recordsTotal = pagereslut.Total,
                recordsFiltered = pagereslut.Total,
                data = pagereslut.Rows
            };
            return Json(json);
        }

        #endregion

        #region 异形件

        public IActionResult Different()
        {
            return View();
        }

        #endregion

        #region 退信

        public IActionResult Back()
        {
            return View();
        }


        public async Task<IActionResult> GetBackLetters(int draw, int start, int length)
        {
            var keywords = Request.Query["search[value]"];
            var pageSearchDto = new PageSearchCommonModel
            {
                DepartmentId = CurrentUser.DepartmentId,
                Keywords = keywords,
                Take = length,
                Skip = start
            };
            var pagereslut = await _letterAppService.GetBackLetters(pageSearchDto);
            var json = new
            {
                draw,
                recordsTotal = pagereslut.Total,
                recordsFiltered = pagereslut.Total,
                data = pagereslut.Rows
            };
            return Json(json);
        }

        public async Task<IActionResult> SearchBackLetters(int draw, int start, int length)
        {
            var pageSearchDto = new PageSearchCommonModel
            {
                DepartmentId = CurrentUser.DepartmentId,
                Keywords = Request.Query["letterNo"],
                Take = length,
                Skip = start
            };

            var pagereslut = string.IsNullOrWhiteSpace(pageSearchDto.Keywords) ? new PageResultModel<LetterBackListDto> { Rows = new List<LetterBackListDto>() } : await _letterAppService.GetBackLettersForSearch(pageSearchDto);
            var json = new
            {
                draw,
                recordsTotal = pagereslut.Total,
                recordsFiltered = pagereslut.Total,
                data = pagereslut.Rows
            };
            return Json(json);
        }


        [HttpPost]
        public async Task<IActionResult> BackLetter(int id)
        {
            var result = await _letterAppService.BackLetter(id, CurrentUser.DepartmentId, CurrentUser.UserId);
            return Json(result);
        }

        #endregion

        #region 勘误

        public IActionResult Cancel()
        {
            return View();
        }

        public async Task<IActionResult> GetCancelLetters(int draw, int start, int length)
        {
            var keywords = Request.Query["search[value]"];
            var pageSearchDto = new PageSearchCommonModel
            {
                DepartmentId = CurrentUser.DepartmentId,
                UserId = CurrentUser.UserId,
                Keywords = keywords,
                Take = length,
                Skip = start
            };
            var pagereslut = await _letterAppService.GetCancelLetters(pageSearchDto);
            var json = new
            {
                draw,
                recordsTotal = pagereslut.Total,
                recordsFiltered = pagereslut.Total,
                data = pagereslut.Rows
            };
            return Json(json);
        }

        public async Task<IActionResult> SearchCancelLetters(int draw, int start, int length)
        {
            var pageSearchDto = new PageSearchCommonModel
            {
                DepartmentId = CurrentUser.DepartmentId,
                Keywords = Request.Query["letterNo"],
                Take = length,
                Skip = start
            };

            var pagereslut = string.IsNullOrWhiteSpace(pageSearchDto.Keywords) ? new PageResultModel<LetterBackListDto> { Rows = new List<LetterBackListDto>() } : await _letterAppService.GetBackLettersForSearch(pageSearchDto);
            var json = new
            {
                draw,
                recordsTotal = pagereslut.Total,
                recordsFiltered = pagereslut.Total,
                data = pagereslut.Rows
            };
            return Json(json);
        }

        [HttpPost]
        public async Task<IActionResult> CancelLetter(int id, int applicantId)
        {
            var result = await _letterAppService.CancelLetter(id, CurrentUser.DepartmentId, CurrentUser.UserId, applicantId);
            return Json(result);
        }

        public async Task<IActionResult> GetUsers()
        {
            var users = await _userAppService.GetUsersAsync();
            var json = users.Select(a => new SelectViewModel
            {
                Id = a.Id,
                Text = a.DisplayName
            });
            return Json(json);
        }

        #endregion

        #region 统计

        public IActionResult Statistics()
        {
            return View();
        }

        #endregion

        #region 分拣

        public IActionResult Sorting()
        {
            return View();
        }

        #endregion

        #region 通用

        public async Task<IActionResult> GetDetailSelect(int id)
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

        public async Task<IActionResult> GetDepartments()
        {
            var departments = await _departmentAppService.GetAllAsync();
            var json = departments.Select(a => new SelectViewModel
            {
                Id = a.Id,
                Text = a.FullName
            }).ToList();
            json.Insert(0, new SelectViewModel
            {
                Id = 0,
                Text = "请选择"
            });
            return Json(json);
        }

        #endregion
    }
}
