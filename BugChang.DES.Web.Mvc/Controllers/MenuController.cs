﻿using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Application.Menus;
using BugChang.DES.Application.Menus.Dtos;
using BugChang.DES.Web.Mvc.Models.Common;
using Microsoft.AspNetCore.Mvc;

namespace BugChang.DES.Web.Mvc.Controllers
{
    public class MenuController : BaseController
    {
        private readonly IMenuAppService _menuAppService;

        public MenuController(IMenuAppService menuAppService)
        {
            _menuAppService = menuAppService;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Edit(MenuEditDto menu)
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetTreeData(int? parentId)
        {
            var menus = await _menuAppService.GetAllAsync(parentId);
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
                    Id = null,
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
            var pagereslut = await _menuAppService.GetPagingAysnc(parentId, length, start, keywords);
            var json = new
            {
                draw,
                recordsTotal = pagereslut.Total,
                recordsFiltered = pagereslut.Total,
                data = pagereslut.Rows
            };
            return Json(json);
        }
    }
}