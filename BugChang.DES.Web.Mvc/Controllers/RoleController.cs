using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Application.Menus;
using BugChang.DES.Application.Roles;
using BugChang.DES.Application.Roles.Dtos;
using BugChang.DES.Core.Commons;
using BugChang.DES.Web.Mvc.Models.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;


namespace BugChang.DES.Web.Mvc.Controllers
{
    public class RoleController : BaseController
    {
        private readonly IRoleAppService _roleAppService;
        private readonly IMenuAppService _menuAppService;

        public RoleController(IRoleAppService roleAppService, IMenuAppService menuAppService)
        {
            _roleAppService = roleAppService;
            _menuAppService = menuAppService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> EditRoleModal(int id)
        {
            var model = await _roleAppService.GetForEditByIdAsync(id);
            return PartialView("_EditRoleModal", model);
        }

        public async Task<IActionResult> EditRoleMenuModal(int id)
        {
            return PartialView("_EditRoleMenuModal");
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
            var result = await _roleAppService.DeleteByIdAsync(id);
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetTreeForRoleMenu()
        {
            var menus = await _menuAppService.GetAllAsync();
            var treedata = menus.Select(a => new SimpleTreeViewModel
            {
                Id = a.Id,
                Name = a.Name,
                ParentId = a.ParentId
            }).ToList();

            return Json(treedata);
        }
    }
}
