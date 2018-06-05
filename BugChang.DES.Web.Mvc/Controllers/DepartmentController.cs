using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BugChang.DES.Application.Departments;
using BugChang.DES.Application.Departments.Dtos;
using BugChang.DES.Core.Common;
using BugChang.DES.Web.Mvc.Filters;
using BugChang.DES.Web.Mvc.Models.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;


namespace BugChang.DES.Web.Mvc.Controllers
{
    public class DepartmentController : BaseController
    {
        private readonly IDepartmentAppService _departmentAppService;

        public DepartmentController(IDepartmentAppService departmentAppService)
        {
            _departmentAppService = departmentAppService;
        }

        [ServiceFilter(typeof(MenuFilter))]
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> EditDepartmentModal(int id)
        {
            var model = await _departmentAppService.GetAsync(id);
            return PartialView("_EditDepartmentModal", model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(DepartmentEditDto department)
        {
            var result = new ResultEntity();
            if (ModelState.IsValid)
            {
                result = await _departmentAppService.AddOrUpdateAsync(department);
                return Json(result);
            }
            result.Message = ModelState.Values
                .FirstOrDefault(a => a.ValidationState == ModelValidationState.Invalid)?.Errors.FirstOrDefault()
                ?.ErrorMessage;

            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetTreeData(int? parentId)
        {
            var departments = await _departmentAppService.GetAllAsync(parentId);
            var treedata = departments.Select(a => new TreeViewModel
            {
                Id = a.Id,
                Name = a.Name,
                IsParent = a.Children.Count > 0
            }).ToList();
            if (parentId == null)
            {
                var treeDataWithRoot = new TreeViewModel
                {
                    Id = null,
                    Name = "机构树",
                    Open = true,
                    IsParent = true,
                    Children = treedata
                };
                return Json(treeDataWithRoot);
            }

            return Json(treedata);
        }

        public async Task<JsonResult> GetListForTable(int draw, int start, int length, int? parentId)
        {
            var keywords = Request.Query["search[value]"];
            var pagereslut = await _departmentAppService.GetPagingAysnc(parentId, length, start, keywords);
            var json = new
            {
                draw,
                recordsTotal = pagereslut.Total,
                recordsFiltered = pagereslut.Total,
                data = pagereslut.Rows
            };
            return Json(json);

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

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _departmentAppService.DeleteAsync(id);
            return Json(result);
        }
    }
}
