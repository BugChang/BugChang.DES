using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Application.Clients;
using BugChang.DES.Application.Departments;
using BugChang.DES.Application.Groups;
using BugChang.DES.Application.Letters;
using BugChang.DES.Application.Letters.Dtos;
using BugChang.DES.Application.Places;
using BugChang.DES.Application.Users;
using BugChang.DES.Core.Commons;
using BugChang.DES.Core.Exchanges.Channel;
using BugChang.DES.Core.Exchanges.Places;
using BugChang.DES.Core.Letters;
using BugChang.DES.Web.Mvc.Filters;
using BugChang.DES.Web.Mvc.Models.Common;
using BugChang.DES.Web.Mvc.Models.Letter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace BugChang.DES.Web.Mvc.Controllers
{
    [ServiceFilter(typeof(RefererFilter))]
    public class LetterController : BaseController
    {
        private readonly ILetterAppService _letterAppService;
        private readonly IDepartmentAppService _departmentAppService;
        private readonly IGroupAppService _groupAppService;
        private readonly IUserAppService _userAppService;
        private readonly IClientAppService _clientAppService;
        private readonly IPlaceAppService _placeAppService;
        public LetterController(ILetterAppService letterAppService, IDepartmentAppService departmentAppService, IGroupAppService groupAppService, IUserAppService userAppService, IClientAppService clientAppService, IPlaceAppService placeAppService)
        {
            _letterAppService = letterAppService;
            _departmentAppService = departmentAppService;
            _groupAppService = groupAppService;
            _userAppService = userAppService;
            _clientAppService = clientAppService;
            _placeAppService = placeAppService;
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

        public async Task<IActionResult> GetSendBarcode(int id)
        {
            var letter = await _letterAppService.GetReceiveBarcode(id);
            return Json(letter);
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
            var placeId = await _placeAppService.GetPlaceId(CurrentUser.UserId);
            var pageSearchDto = new PageSearchCommonModel
            {
                DepartmentId = CurrentUser.DepartmentId,
                Keywords = Request.Query["letterNo"],
                Take = length,
                Skip = start,
                PlaceId = placeId
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
            var placeId = await _placeAppService.GetPlaceId(CurrentUser.UserId);
            var pageSearchDto = new PageSearchCommonModel
            {
                DepartmentId = CurrentUser.DepartmentId,
                Keywords = Request.Query["letterNo"],
                Take = length,
                Skip = start,
                PlaceId = placeId
            };



            var pageReslut = string.IsNullOrWhiteSpace(pageSearchDto.Keywords) ? new PageResultModel<LetterCancelListDto> { Rows = new List<LetterCancelListDto>() } : await _letterAppService.GetCancelLettersForSearch(pageSearchDto);
            var json = new
            {
                draw,
                recordsTotal = pageReslut.Total,
                recordsFiltered = pageReslut.Total,
                data = pageReslut.Rows
            };
            return Json(json);
        }

        [HttpPost]
        public async Task<IActionResult> CancelLetter(int id)
        {
            var result = await _letterAppService.CancelLetter(id, CurrentUser.DepartmentId, CurrentUser.UserId, 0);
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
            return View(new Dictionary<string, int>());
        }

        public async Task<IActionResult> StatisticsDepartment(int id, DateTime beginDate, DateTime endDate)
        {
            var model = await _letterAppService.GetDepartmentStatistics(id, beginDate, endDate);
            return PartialView("_StatisticsDepartment", model);
        }

        public async Task<IActionResult> StatisticsPlace(DateTime beginDate, DateTime endDate)
        {
            var dictionary = await _letterAppService.GetPlaceStatistics(beginDate, endDate);
            return View("Statistics", dictionary);
        }

        public async Task<IActionResult> GetDepartmentsForStatistics()
        {
            var departments = await _departmentAppService.GetAllAsync();
            var json = departments.Select(a => new SelectViewModel
            {
                Id = a.Id,
                Text = a.FullName
            });
            return Json(json);
        }

        #endregion

        #region 分拣

        public IActionResult Sorting()
        {
            ViewBag.CurrentUser = CurrentUser;
            return View();
        }

        /// <returns></returns>
        public async Task<IActionResult> GetTcjhNoSortingLetters(string search, int limit, int offset)
        {
            var pagereslut = await _letterAppService.GetNoSortingLetters(EnumChannel.同城交换);
            return Json(pagereslut);
        }

        public async Task<IActionResult> GetJytxNoSortingLetters()
        {
            var pagereslut = await _letterAppService.GetNoSortingLetters(EnumChannel.机要通信);
            return Json(pagereslut);
        }

        public async Task<IActionResult> GetZsNoSortingLetters()
        {
            var pagereslut = await _letterAppService.GetNoSortingLetters(EnumChannel.直送);
            return Json(pagereslut);
        }

        public async Task<IActionResult> CreateTcjhList(string letterIds)
        {
            var lstLetterId = letterIds.Split(',').ToList().Select(x => Convert.ToInt32(x)).ToList();
            var result = await _letterAppService.CreateSortingList(EnumChannel.同城交换, lstLetterId, CurrentUser.UserId);
            return Json(result);
        }


        public async Task<IActionResult> CreateJytxList(string letterIds)
        {
            var lstLetterId = letterIds.Split(',').ToList().Select(x => Convert.ToInt32(x)).ToList();
            var result = await _letterAppService.CreateSortingList(EnumChannel.机要通信, lstLetterId, CurrentUser.UserId);
            return Json(result);
        }

        public async Task<IActionResult> CreateZsList(string letterIds)
        {
            var lstLetterId = letterIds.Split(',').ToList().Select(x => Convert.ToInt32(x)).ToList();
            var result = await _letterAppService.CreateSortingList(EnumChannel.直送, lstLetterId, CurrentUser.UserId);
            return Json(result);
        }

        public async Task<IActionResult> Change2Jytx(int id)
        {
            var result = await _letterAppService.Change2Jytx(id);
            return Json(result);
        }

        public async Task<IActionResult> GetWriteCpuCardData(int listId)
        {
            var result = await _letterAppService.GetWriteCpuCardData(listId);
            return Json(result);
        }

        public async Task<IActionResult> GetSortingListDetails(int listId)
        {
            var letters = await _letterAppService.GetSortListDetails(listId);
            return Json(letters);
        }

        public async Task<IActionResult> GetLetterIdByBarcodeNo(string barcodeNo)
        {
            var letterId = await _letterAppService.GetLetterIdByBarcodeNo(barcodeNo);
            return Json(letterId);
        }

        [HttpPost]
        public async Task<IActionResult> SortingPrintTcjh(int id)
        {
            var model = new PrintSortingModel
            {
                SortingList = await _letterAppService.GetSortingList(id),
                LetterSortings = await _letterAppService.GetSortListDetails(id)
            };
            return PartialView("_Sorting_Print_Tcjh", model);
        }

        [HttpPost]
        public async Task<IActionResult> SortingPrintZs(int id)
        {
            var model = new PrintSortingModel
            {
                SortingList = await _letterAppService.GetSortingList(id),
                LetterSortings = await _letterAppService.GetSortListDetails(id)
            };
            return PartialView("_Sorting_Print_Zs", model);
        }

        [HttpPost]
        public async Task<IActionResult> SortingPrintJytx(int id)
        {
            var model = new PrintSortingModel
            {
                SortingList = await _letterAppService.GetSortingList(id),
                LetterSortings = await _letterAppService.GetSortListDetails(id)
            };

            var left = model.LetterSortings.Count % 15;
            if (left != 0)
            {
                for (int i = 0; i < left; i++)
                {
                    model.LetterSortings.Add(new LetterSortingDto());
                }
            }
            return PartialView("_Sorting_Print_Jytx", model);
        }

        public async Task<IActionResult> GetSortingLists(int draw, int start, int length)
        {
            var keywords = Request.Query["search[value]"];
            var pageSearchDto = new PageSearchCommonModel
            {
                Keywords = keywords,
                Take = length,
                Skip = start
            };
            var pagereslut = await _letterAppService.GetSortingLists(pageSearchDto);
            var json = new
            {
                draw,
                recordsTotal = pagereslut.Total,
                recordsFiltered = pagereslut.Total,
                data = pagereslut.Rows
            };
            return Json(json);
        }

        public IActionResult SortingList()
        {
            return View();
        }

        public IActionResult SortingListDetail(int id)
        {
            ViewBag.ListId = id;
            return PartialView("_SortingListDetail");
        }


        public async Task<IActionResult> GetSortingListDetailsForTable(int draw, int start, int length, int listId)
        {
            var letters = await _letterAppService.GetSortListDetails(listId);
            var json = new
            {
                draw,
                recordsTotal = letters.Count,
                recordsFiltered = letters.Count,
                data = letters
            };
            return Json(json);
        }
        #endregion

        #region 核销

        public IActionResult Check()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Check(LetterCheckModel model)
        {
            var result = new ResultEntity();
            var client = await _clientAppService.GetClient(model.DeviceCode);
            if (client == null)
            {
                result.Message = "未登记的客户端，无法使用此功能";
            }
            else
            {
                var placeId = client.PlaceId;
                result = await _letterAppService.GetCheckInfo(CurrentUser.UserId, placeId, model.BeginTime, model.EndTime);
            }
            return Json(result);
        }

        public async Task<IActionResult> GetCheckLetters(int draw, int start, int length, string deviceCode, DateTime beginTime, DateTime endTime)
        {
            var client = await _clientAppService.GetClient(deviceCode);
            var pageSearchDto = new PageSearchDetailModel
            {
                Take = length,
                Skip = start,
                UserId = CurrentUser.UserId,
                PlaceId = client.PlaceId,
                BeginTime = beginTime,
                EndTime = endTime
            };

            var pagereslut = await _letterAppService.GetCheckLetters(pageSearchDto);
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

        #region 外收转内发

        public IActionResult Out2Inside()
        {
            return View();
        }

        public async Task<IActionResult> Out2InsideLetters(int draw, int start, int length)
        {
            var keywords = Request.Query["search[value]"];
            var pageSearchDto = new PageSearchCommonModel
            {
                Keywords = keywords,
                Take = length,
                Skip = start,
            };

            var pagereslut = await _letterAppService.Out2InsideLetters(pageSearchDto);
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

        #region 通用

        public async Task<IActionResult> GetDetailSelect(int id)
        {
            var departments = await _departmentAppService.GetAllAsync();
            var groupDetails = await _groupAppService.GetGroupDetails(id);
            departments = departments.Where(a => groupDetails.Select(b => b.DepartmentId).Contains(a.Id)).OrderBy(a => a.Sort).ToList();
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

        #region 流转记录

        public async Task<IActionResult> ExchangeLog(int id)
        {
            var barcodeNo = await _letterAppService.GetBarcodeNoByLetterId(id);
            ViewBag.BarcodeNo = barcodeNo;
            return PartialView("_ExchangeLog");
        }


        public async Task<IActionResult> GetExchangeLogs(int draw, int start, int length, string barcodeNo)
        {
            var letters = await _letterAppService.GetExchangeLogs(barcodeNo);
            var json = new
            {
                draw,
                recordsTotal = letters.Count,
                recordsFiltered = letters.Count,
                data = letters
            };
            return Json(json);
        }

        #endregion

        #region 信件查询

        public IActionResult Search()
        {
            return View();
        }

        public async Task<IActionResult> GetSearchList(int draw, int start, int length)
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

            if (!string.IsNullOrWhiteSpace(Request.Query["receiveDepartmentId"]))
            {
                pageSearchDto.ReceiveDepartmentId = Convert.ToInt32(Request.Query["receiveDepartmentId"]);
            }

            pageSearchDto.ShiJiNo = Request.Query["shiJiNo"];
            pageSearchDto.LetterNo = Request.Query["letterNo"];
            var pagereslut = await _letterAppService.GetSearchLetters(pageSearchDto);
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
    }
}
