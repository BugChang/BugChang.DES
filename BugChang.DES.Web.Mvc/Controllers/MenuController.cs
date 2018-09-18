using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Application.Commons;
using BugChang.DES.Application.Menus;
using BugChang.DES.Application.Menus.Dtos;
using BugChang.DES.Core.Commons;
using BugChang.DES.Web.Mvc.Filters;
using BugChang.DES.Web.Mvc.Models.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BugChang.DES.Web.Mvc.Controllers
{
    [ServiceFilter(typeof(RefererFilter))]
    public class MenuController : BaseController
    {
        private readonly IMenuAppService _menuAppService;

        public MenuController(IMenuAppService menuAppService)
        {
            _menuAppService = menuAppService;
        }


        [ServiceFilter(typeof(MenuFilter))]
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> EditMenuModal(int id)
        {
            var model = await _menuAppService.GetForEditByIdAsync(id);
            return PartialView("_EditMenuModal", model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(MenuEditDto menu)
        {
            var result = new ResultEntity();
            if (ModelState.IsValid)
            {
                menu.ParentId = menu.ParentId == 0 ? null : menu.ParentId;
                menu.SetCreateOrUpdateInfo(CurrentUser.UserId);
                result = await _menuAppService.AddOrUpdateAsync(menu);
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
            var tempParentId = parentId == 0 ? null : parentId;
            var menus = await _menuAppService.GetAllAsync(tempParentId);
            var treedata = menus.Select(a => new TreeViewModel
            {
                Id = a.Id,
                Name = a.Name,
                CustomData = a.Url,
                IsParent = a.Items.Count > 0
            }).ToList();
            if (parentId == null)
            {
                var treeDataWithRoot = new TreeViewModel
                {
                    Id = 0,
                    Name = "菜单树",
                    Open = true,
                    IsParent = true,
                    Children = treedata
                };
                return Json(treeDataWithRoot);
            }

            return Json(treedata);
        }

        public async Task<IActionResult> GetListForSelect()
        {
            var departments = await _menuAppService.GetAllAsync();
            departments.Insert(0, new MenuListDto { Id = 0, Name = "无" });
            var json = departments.Select(a => new SelectViewModel
            {
                Id = a.Id,
                Text = a.Name
            });
            return Json(json);
        }

        public async Task<JsonResult> GetListForTable(int draw, int start, int length, int? parentId)
        {
            var keywords = Request.Query["search[value]"];
            var pageSearchDto = new PageSearchCommonModel
            {
                Keywords = keywords,
                ParentId = parentId == 0 ? null : parentId,
                Take = length,
                Skip = start
            };
            var pagereslut = await _menuAppService.GetPagingAysnc(pageSearchDto);
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
            var result = await _menuAppService.DeleteByIdAsync(id, CurrentUser.UserId);
            return Json(result);
        }
    }
}
