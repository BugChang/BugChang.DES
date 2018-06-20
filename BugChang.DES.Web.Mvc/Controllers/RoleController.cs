using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Application.Menus;
using BugChang.DES.Application.Operations;
using BugChang.DES.Application.Roles;
using BugChang.DES.Application.Roles.Dtos;
using BugChang.DES.Core.Commons;
using BugChang.DES.Web.Mvc.Filters;
using BugChang.DES.Web.Mvc.Models.Common;
using BugChang.DES.Web.Mvc.Models.Role;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;


namespace BugChang.DES.Web.Mvc.Controllers
{
    public class RoleController : BaseController
    {
        private readonly IRoleAppService _roleAppService;
        private readonly IMenuAppService _menuAppService;
        private readonly IOperationAppService _operationAppService;

        public RoleController(IRoleAppService roleAppService, IMenuAppService menuAppService, IOperationAppService operationAppService)
        {
            _roleAppService = roleAppService;
            _menuAppService = menuAppService;
            _operationAppService = operationAppService;
        }

        [ServiceFilter(typeof(MenuFilter))]
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> EditRoleModal(int id)
        {
            var model = await _roleAppService.GetForEditByIdAsync(id);
            return PartialView("_EditRoleModal", model);
        }

        public IActionResult EditRoleMenuModal(int id)
        {
            ViewBag.RoleId = id;
            return PartialView("_EditRoleMenuModal");
        }

        public IActionResult EditRoleOperationModal(int id)
        {
            ViewBag.RoleId = id;
            return PartialView("_EditRoleOperationModal");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(RoleEditDto role)
        {
            var result = new ResultEntity();
            if (ModelState.IsValid)
            {
                role.SetCreateOrUpdateInfo(CurrentUserId);
                result = await _roleAppService.AddOrUpdateAsync(role);
                return Json(result);
            }
            result.Message = ModelState.Values
                .FirstOrDefault(a => a.ValidationState == ModelValidationState.Invalid)?.Errors.FirstOrDefault()
                ?.ErrorMessage;

            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> EditRoleMenu(int roleId, string strMenuId)
        {
            var result = new ResultEntity();
            if (roleId == 0)
            {
                result.Message = "角色Id有误";
            }
            else if (strMenuId == null)
            {
                result.Message = "菜单分配有误";
            }
            else
            {
                var lstMenuId = strMenuId.Split(',').ToList().Select(x => Convert.ToInt32(x)).ToList();
                result = await _roleAppService.EditRoleMenu(roleId, lstMenuId);
            }

            return Json(result);
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
            var pagereslut = await _roleAppService.GetPagingAysnc(pageSearchDto);
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
            var result = await _roleAppService.DeleteByIdAsync(id, CurrentUserId);
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetTreeForRoleMenu(int id)
        {
            var menus = await _menuAppService.GetAllAsync();
            var roleMenus = await _menuAppService.GetAllByRoleIdAsync(id);
            var treedata = menus.Select(a => new SimpleTreeViewModel
            {
                Id = a.Id,
                Name = a.Name,
                ParentId = a.ParentId,
                Checked = roleMenus.Any(b => b.Id == a.Id)
            }).ToList();

            return Json(treedata);
        }

        [HttpGet]
        public async Task<IActionResult> GetTreeForRoleOperation(int id)
        {
            var roleMenus = await _menuAppService.GetAllByRoleIdAsync(id);
            var treedata = roleMenus.Select(a => new SimpleTreeViewModel
            {
                Id = a.Id,
                Name = a.Name,
                ParentId = a.ParentId,
                CustomData = a.Url
            }).ToList();

            return Json(treedata);
        }


        public IActionResult GetOperationsByUrl(string url, int roleId)
        {
            var operations = _operationAppService.GetOperationsByUrl(url, out string module);

            var lstRoleId = new List<int> { roleId };
            var roleOperationCodes = _roleAppService.GetRoleOperationCodes(module, lstRoleId);
            var json = operations.Select(a => new RoleOperationViewModel
            {
                Checked = roleOperationCodes.Contains(a.Code),
                OperationCode = a.Code,
                OperationName = a.Name
            }).ToList();
            return Json(json);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRoleOperation(int roleId, string operationCode)
        {
            var result = await _roleAppService.DeleteRoleOperation(roleId, operationCode);
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddRoleOperation(int roleId, string operationCode)
        {
            var result = await _roleAppService.AddRoleOperation(roleId, operationCode);
            return Json(result);
        }


        [HttpGet]
        public IActionResult GetRoleOperations(string module)
        {
            var roles = HttpContext.User.FindAll("RoleId").ToList();
            var lstRoleId = new List<int>();
            foreach (var roleClaim in roles)
            {
                lstRoleId.Add(Convert.ToInt32(roleClaim.Value));
            }

            var operations = _roleAppService.GetRoleOperationCodes(module, lstRoleId);
            return Json(operations);
        }
    }
}
