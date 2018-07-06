using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Application.Channels;
using BugChang.DES.Application.Departments;
using BugChang.DES.Application.ExchangeObjects;
using BugChang.DES.Application.Users;
using BugChang.DES.Core.Exchanges.ExchangeObjects;
using BugChang.DES.Web.Mvc.Models.Common;
using Microsoft.AspNetCore.Mvc;

namespace BugChang.DES.Web.Mvc.Controllers
{
    public class ExchangeObjectController : BaseController
    {

        private readonly IChannelAppService _channelAppService;
        private readonly IExchangeObjectAppService _exchangeObjectAppService;
        private readonly IDepartmentAppService _departmentAppService;
        private readonly IUserAppService _userAppService;

        public ExchangeObjectController(IChannelAppService channelAppService, IExchangeObjectAppService exchangeObjectAppService, IDepartmentAppService departmentAppService, IUserAppService userAppService)
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
    }
}
