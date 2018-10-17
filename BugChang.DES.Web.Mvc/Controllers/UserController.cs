using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Application.Departments;
using BugChang.DES.Application.Roles;
using BugChang.DES.Application.Users;
using BugChang.DES.Application.Users.Dtos;
using BugChang.DES.Core.Commons;
using BugChang.DES.Web.Mvc.Filters;
using BugChang.DES.Web.Mvc.Models.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BugChang.DES.Web.Mvc.Controllers
{
    [ServiceFilter(typeof(RefererFilter))]
    public class UserController : BaseController
    {
        private readonly IUserAppService _userAppService;
        private readonly IRoleAppService _roleAppService;
        private readonly IDepartmentAppService _departmentAppService;


        public UserController(IUserAppService userAppService, IDepartmentAppService departmentAppService, IRoleAppService roleAppService)
        {
            _userAppService = userAppService;
            _departmentAppService = departmentAppService;
            _roleAppService = roleAppService;
        }

        [ServiceFilter(typeof(MenuFilter))]
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

        public IActionResult EditUserRoleModal(int id)
        {
            ViewBag.UserId = id;
            return PartialView("_EditUserRoleModal");
        }

        [HttpPost]
        [TypeFilter(typeof(OperationFilter),
            Arguments = new object[] { "User.Create" })]
        public async Task<IActionResult> Create(UserEditDto user)
        {
            if (user.Id > 0)
            {
                return Json(new ResultEntity { Message = "请求数据有误，新增数据非0主键" });
            }
            return await CreateOrUpdate(user);
        }

        [HttpPost]
        [TypeFilter(typeof(OperationFilter),
            Arguments = new object[] { "User.Edit" })]
        public async Task<IActionResult> Edit(UserEditDto user)
        {
            if (user.Id <= 0)
            {
                return Json(new ResultEntity { Message = "请求数据有误，修改数据空主键" });
            }

            if ("sysadmin,secadmin,audadmin".Contains(user.UserName))
            {
                return Json(new ResultEntity { Message = "系统预设角色不允许更改" });
            }
            return await CreateOrUpdate(user);
        }

        private async Task<IActionResult> CreateOrUpdate(UserEditDto user)
        {
            var result = new ResultEntity();
            if (ModelState.IsValid)
            {

                user.SetCreateOrUpdateInfo(CurrentUser.UserId);
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
            var pageSearchDto = new PageSearchCommonModel
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
            var result = await _userAppService.DeleteByIdAsync(id, CurrentUser.UserId);
            return Json(result);
        }

        public async Task<IActionResult> GetRolesForSelect()
        {
            var roles = await _roleAppService.GetAllRoles();
            var json = roles.Select(a => new SelectViewModel
            {
                Id = a.Id,
                Text = a.Name
            });
            return Json(json);
        }

        public async Task<IActionResult> GetUserRoles(int draw, int id)
        {

            var userRoles = await _userAppService.GetUserRoles(id);
            var json = new
            {
                draw,
                recordsTotal = userRoles.Count,
                recordsFiltered = userRoles.Count,
                data = userRoles
            };
            return Json(json);
        }

        [HttpPost]
        public async Task<IActionResult> AddUserRole(int userId, int roleId)
        {
            var result = await _userAppService.AddUserRole(userId, roleId, CurrentUser.UserId);
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUserRole(int userId, int roleId)
        {
            var result = await _userAppService.DeleteUserRole(userId, roleId, CurrentUser.UserId);
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeUserEnabled(int id)
        {
            var result = await _userAppService.ChangeUserEnabled(id, CurrentUser.UserId);
            return Json(result);
        }
    }
}
