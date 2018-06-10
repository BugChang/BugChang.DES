using System;
using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Application.Commons;
using BugChang.DES.Application.Departments;
using BugChang.DES.Application.Users;
using BugChang.DES.Application.Users.Dtos;
using BugChang.DES.Core.Commons;
using BugChang.DES.Web.Mvc.Models.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BugChang.DES.Web.Mvc.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUserAppService _userAppService;
        private readonly IDepartmentAppService _departmentAppService;


        public UserController(IUserAppService userAppService, IDepartmentAppService departmentAppService)
        {
            _userAppService = userAppService;
            _departmentAppService = departmentAppService;

        }

        public async Task<IActionResult> Index()
        {
            var model = await _userAppService.GetUsersAsync();
            return View(model);
        }

        public async Task<IActionResult> EditUserModal(int id)
        {
            var model = await _userAppService.GetForEditByIdAsync(id);
            return PartialView("_EditUserModal", model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserEditDto user)
        {
            var result = new ResultEntity();
            if (ModelState.IsValid)
            {
                user.SetCreateOrUpdateInfo(CurrentUserId);
                result = await _userAppService.AddOrUpdateAsync(user);
                return Json(result);
            }
            result.Message = ModelState.Values
                .FirstOrDefault(a => a.ValidationState == ModelValidationState.Invalid)?.Errors.FirstOrDefault()
                ?.ErrorMessage;

            return Json(result);
        }


        public async Task<IActionResult> GetListForSelect()
        {
            var departments = await _departmentAppService.GetAllAsync();
            var json = departments.Select(a => new SelectViewModel
            {
                Id = a.Id,
                Text = a.FullName
            });
            return Json(json);
        }

        public async Task<JsonResult> GetListForTable(int draw, int start, int length)
        {
            var keywords = Request.Query["search[value]"];
            var pageSearchDto = new PageSearchModel
            {
                Keywords = keywords,
                Take = length,
                Skip = start
            };
            var pagereslut = await _userAppService.GetPagingAysnc(pageSearchDto);
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
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _userAppService.DeleteByIdAsync(id);
            return Json(result);
        }
    }
}
