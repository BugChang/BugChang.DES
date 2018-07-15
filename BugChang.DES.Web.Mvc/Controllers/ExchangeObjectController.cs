using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Application.Channels;
using BugChang.DES.Application.Departments;
using BugChang.DES.Application.Departments.Dtos;
using BugChang.DES.Application.ExchangeObjects;
using BugChang.DES.Application.ExchangeObjects.Dtos;
using BugChang.DES.Application.Users;
using BugChang.DES.Core.Commons;
using BugChang.DES.Core.Exchanges.ExchangeObjects;
using BugChang.DES.Web.Mvc.Filters;
using BugChang.DES.Web.Mvc.Models.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BugChang.DES.Web.Mvc.Controllers
{
    public class ExchangeObjectController : BaseController
    {

        private readonly IChannelAppService _channelAppService;
        private readonly IExchangeObjectAppService _exchangeObjectAppService;
        private readonly IDepartmentAppService _departmentAppService;
        private readonly IUserAppService _userAppService;

        public ExchangeObjectController(IChannelAppService channelAppService, IExchangeObjectAppService exchangeObjectAppService, IDepartmentAppService departmentAppService,
            IUserAppService userAppService)
        {
            _channelAppService = channelAppService;
            _exchangeObjectAppService = exchangeObjectAppService;
            _departmentAppService = departmentAppService;
            _userAppService = userAppService;
        }

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取所有渠道
        /// </summary>
        /// <returns></returns>
        private IActionResult GetChannels()
        {
            var channels = _channelAppService.GetChannels();
            var json = channels.Select(a => new SelectViewModel
            {
                Id = a.Id,
                Text = a.Name
            });
            return Json(json);
        }

        /// <summary>
        /// 获取所有机构
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> GetDepartments()
        {
            var departments = await _departmentAppService.GetAllAsync();
            var json = departments.Select(a => new SelectViewModel
            {
                Id = a.Id,
                Text = a.Name
            });
            return Json(json);
        }


        /// <summary>
        /// 获取所有用户
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> GetUsers()
        {
            var users = await _userAppService.GetUsersAsync();
            var json = users.Select(a => new SelectViewModel
            {
                Id = a.Id,
                Text = a.DisplayName
            });
            return Json(json);
        }

        /// <summary>
        /// 获取流转对象类型
        /// </summary>
        /// <returns></returns>
        public IActionResult GetObjectTypes()
        {
            var objectTypes = _exchangeObjectAppService.GetObjectTypes();
            var json = objectTypes.Select(a => new SelectViewModel
            {
                Id = a.Id,
                Text = a.Name
            });
            return Json(json);
        }

        /// <summary>
        /// 获取值的列表
        /// </summary>
        /// <param name="objectType">流转对象类型</param>
        /// <returns></returns>
        public async Task<IActionResult> GetValuesByObjectType(int objectType)
        {
            var enumObjectType = (EnumObjectType)objectType;
            switch (enumObjectType)
            {
                case EnumObjectType.渠道:
                    return GetChannels();
                case EnumObjectType.机构:
                    return await GetDepartments();
                case EnumObjectType.人:
                    return await GetUsers();
                default:
                    return new JsonResult(null);
            }
        }

        public async Task<IActionResult> GetExchangeObjects(int draw, int start, int length)
        {
            var keywords = Request.Query["search[value]"];
            var pageSearchDto = new PageSearchModel
            {
                Keywords = keywords,
                Take = length,
                Skip = start
            };
            var pagereslut = await _exchangeObjectAppService.GetPagingAysnc(pageSearchDto);
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
        [TypeFilter(typeof(OperationFilter),
            Arguments = new object[] { "ExchangeObject.Create" })]
        public async Task<IActionResult> Create(ExchangeObjectEditDto exchangeObject)
        {
            if (exchangeObject.Id > 0)
            {
                return Json(new ResultEntity { Message = "请求数据有误，新增数据非0主键" });
            }
            return await CreateOrUpdate(exchangeObject);
        }

        [HttpPost]
        [TypeFilter(typeof(OperationFilter),
            Arguments = new object[] { "ExchangeObject.Edit" })]
        public async Task<IActionResult> Edit(ExchangeObjectEditDto exchangeObject)
        {
            if (exchangeObject.Id <= 0)
            {
                return Json(new ResultEntity { Message = "请求数据有误，修改数据空主键" });
            }
            return await CreateOrUpdate(exchangeObject);
        }

        private async Task<IActionResult> CreateOrUpdate(ExchangeObjectEditDto exchangeObject)
        {
            var result = new ResultEntity();
            if (ModelState.IsValid)
            {
                exchangeObject.SetCreateOrUpdateInfo(CurrentUserId);
                result = await _exchangeObjectAppService.AddOrUpdateAsync(exchangeObject);
                return Json(result);
            }
            result.Message = ModelState.Values
                .FirstOrDefault(a => a.ValidationState == ModelValidationState.Invalid)?.Errors.FirstOrDefault()
                ?.ErrorMessage;

            return Json(result);
        }

        public async Task<IActionResult> EditExchangeObjectModal(int id)
        {
            var model = await _exchangeObjectAppService.GetForEditByIdAsync(id);
            return PartialView("_EditExchangeObjectModal", model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _exchangeObjectAppService.DeleteByIdAsync(id, CurrentUserId);
            return Json(result);
        }

        public async Task<IActionResult> GetParents()
        {
            var objects = await _exchangeObjectAppService.GetAlListAsync();
            var json = objects.Select(a => new SelectViewModel
            {
                Id = a.Id,
                Text = a.Name
            }).ToList();
            json.Insert(0, new SelectViewModel
            {
                Id = 0,
                Text = "无"
            });
            return Json(json);
        }
    }
}
