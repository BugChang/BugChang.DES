using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Application.Clients;
using BugChang.DES.Application.Departments;
using BugChang.DES.Application.Departments.Dtos;
using BugChang.DES.Application.ExchangeObjects;
using BugChang.DES.Application.Places;
using BugChang.DES.Core.Clients;
using BugChang.DES.Core.Commons;
using BugChang.DES.Web.Mvc.Models.Common;
using Microsoft.AspNetCore.Mvc;

namespace BugChang.DES.Web.Mvc.Controllers
{
    public class BillController : BaseController
    {
        private readonly IClientAppService _clientAppService;
        private readonly IExchangeObjectAppService _exchangeObjectAppService;
        private readonly IDepartmentAppService _departmentAppService;
        private readonly IPlaceAppService _placeAppService;

        public BillController(IClientAppService clientAppService, IExchangeObjectAppService exchangeObjectAppService, IDepartmentAppService departmentAppService, IPlaceAppService placeAppService)
        {
            _clientAppService = clientAppService;
            _exchangeObjectAppService = exchangeObjectAppService;
            _departmentAppService = departmentAppService;
            _placeAppService = placeAppService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult List()
        {
            return View();
        }

        public async Task<IActionResult> CheckReceive(string deviceCode)
        {
            var result = new ResultEntity();
            var client = await _clientAppService.GetClient(deviceCode);
            if (client == null)
            {
                result.Message = "未注册的客户端无法使用此功能";
            }
            else
            {
                if (client.ClientType == EnumClientType.个人终端)
                {
                    result.Message = "请在自助终端使用打印功能";
                }
                else
                {
                    var objects = await _exchangeObjectAppService.GetObjects(CurrentUser.UserId, client.PlaceId);
                    if (objects.Count == 0)
                    {
                        result.Message = "暂无清单";
                    }
                    else
                    {
                        result.Success = true;
                        result.Data = objects.Select(a => new SelectViewModel
                        {
                            Id = a.Id,
                            Text = a.Name
                        }).Distinct();
                    }

                }
            }

            return Json(result);
        }

        public async Task<IActionResult> CheckSend(string deviceCode)
        {
            var result = new ResultEntity();
            var departments = new List<DepartmentEditDto>();
            var department = await _departmentAppService.GetForEditByIdAsync(CurrentUser.DepartmentId);
            departments.Add(department);
            var client = await _clientAppService.GetClient(deviceCode);
            if (client == null)
            {
                result.Message = "未注册的客户端无法使用此功能";
            }
            else
            {
                if (client.ClientType == EnumClientType.个人终端)
                {
                    result.Message = "请在自助终端使用打印功能";
                }
                else
                {
                    var isPlaceWarden = await _placeAppService.IsPlaceWarden(CurrentUser.UserId, client.PlaceId);
                    if (isPlaceWarden)
                    {
                        var place = await _placeAppService.GetForEditByIdAsync(client.PlaceId);
                        var placeDepartment = await _departmentAppService.GetForEditByIdAsync(place.DepartmentId);
                        departments.Add(placeDepartment);
                    }

                    result.Success = true;
                    result.Data = departments.Select(a => new SelectViewModel
                    {
                        Id = a.Id,
                        Text = a.Name
                    }).Distinct();
                }
            }
            return Json(result);
        }

        public IActionResult CheckSendAndReceive(string deviceCode)
        {
            return Json(null);
        }

    }
}
