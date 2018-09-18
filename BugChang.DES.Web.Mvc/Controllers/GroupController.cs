using System;
using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Application.Departments;
using BugChang.DES.Application.Groups;
using BugChang.DES.Application.Groups.Dtos;
using BugChang.DES.Core.Commons;
using BugChang.DES.Web.Mvc.Filters;
using BugChang.DES.Web.Mvc.Models.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BugChang.DES.Web.Mvc.Controllers
{
    [ServiceFilter(typeof(RefererFilter))]
    public class GroupController : BaseController
    {
        private readonly IGroupAppService _groupAppService;
        private readonly IDepartmentAppService _departmentAppService;

        public GroupController(IGroupAppService groupAppService, IDepartmentAppService departmentAppService)
        {
            _groupAppService = groupAppService;
            _departmentAppService = departmentAppService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetGroupTypes()
        {
            var groupTypes = _groupAppService.GetGroupTypes();
            var json = groupTypes.Select(a => new SelectViewModel
            {
                Id = a.Id,
                Text = a.Name
            });
            return Json(json);
        }

        [HttpPost]
        [TypeFilter(typeof(OperationFilter),
            Arguments = new object[] { "Group.Create" })]
        public async Task<IActionResult> Create(GroupEditDto group)
        {
            if (group.Id > 0)
            {
                return Json(new ResultEntity { Message = "请求数据有误，新增数据非0主键" });
            }
            return await CreateOrUpdate(group);
        }

        [HttpPost]
        [TypeFilter(typeof(OperationFilter),
            Arguments = new object[] { "Group.Edit" })]
        public async Task<IActionResult> Edit(GroupEditDto group)
        {
            if (group.Id <= 0)
            {
                return Json(new ResultEntity { Message = "请求数据有误，修改数据空主键" });
            }
            return await CreateOrUpdate(group);
        }

        private async Task<IActionResult> CreateOrUpdate(GroupEditDto group)
        {
            var result = new ResultEntity();
            if (ModelState.IsValid)
            {

                group.SetCreateOrUpdateInfo(CurrentUser.UserId);
                result = await _groupAppService.AddOrUpdateAsync(group);
                return Json(result);
            }
            result.Message = ModelState.Values
                .FirstOrDefault(a => a.ValidationState == ModelValidationState.Invalid)?.Errors.FirstOrDefault()
                ?.ErrorMessage;

            return Json(result);
        }

        public async Task<IActionResult> GetGroups(int draw, int start, int length)
        {
            var keywords = Request.Query["search[value]"];
            var pageSearchDto = new PageSearchCommonModel
            {
                Keywords = keywords,
                Take = length,
                Skip = start
            };
            var pagereslut = await _groupAppService.GetPagingAysnc(pageSearchDto);
            var json = new
            {
                draw,
                recordsTotal = pagereslut.Total,
                recordsFiltered = pagereslut.Total,
                data = pagereslut.Rows
            };
            return Json(json);
        }

        public async Task<IActionResult> EditGroupModal(int id)
        {
            var barcodeRule = await _groupAppService.GetForEditByIdAsync(id);
            return PartialView("_EditGroupModal", barcodeRule);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var result = await _groupAppService.DeleteByIdAsync(id, CurrentUser.UserId);
            return Json(result);
        }

        public async Task<IActionResult> GetGroupDetails(int id)
        {
            var departments = await _departmentAppService.GetAllAsync();
            var groupDetails = await _groupAppService.GetGroupDetails(id);
            var json = departments.Select(a => new SimpleTreeViewModel
            {
                Id = a.Id,
                ParentId = a.ParentId,
                Name = a.Name,
                Checked = groupDetails.Any(b => b.DepartmentId == a.Id)
            });
            return Json(json);
        }

        public IActionResult AssignDetailModal(int id)
        {
            ViewBag.GroupId = id;
            return PartialView("_AssignDetailModal");
        }

        [HttpPost]
        [TypeFilter(typeof(OperationFilter),
            Arguments = new object[] { "Group.AssignDetail" })]
        public async Task<IActionResult> AssignDetail(int groupId, string strDetailId)
        {
            var lstDepartmentId = strDetailId.Split(',').ToList().Select(x => Convert.ToInt32(x)).ToList();
            var result=await _groupAppService.AssignDetail(groupId, lstDepartmentId, CurrentUser.UserId);
            return Json(result);
        }
    }
}
