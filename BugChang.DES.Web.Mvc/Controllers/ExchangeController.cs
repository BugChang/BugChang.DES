using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Application.Departments;
using BugChang.DES.Application.Places;
using BugChang.DES.Application.Places.Dtos;
using BugChang.DES.Core.Commons;
using BugChang.DES.Web.Mvc.Filters;
using BugChang.DES.Web.Mvc.Models.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BugChang.DES.Web.Mvc.Controllers
{
    public class ExchangeController : BaseController
    {

        private readonly IPlaceAppService _placeAppService;
        private readonly IDepartmentAppService _departmentAppService;

        public ExchangeController(IPlaceAppService placeAppService, IDepartmentAppService departmentAppService)
        {
            _placeAppService = placeAppService;
            _departmentAppService = departmentAppService;
        }

        public IActionResult Box()
        {
            return View();
        }

        public IActionResult Place()
        {
            return View();
        }

        public async  Task<IActionResult> EditPlaceModal(int id)
        {
            var model = await _placeAppService.GetForEditByIdAsync(id);
            return PartialView("_EditPlaceModal", model);
        }

        [HttpPost]
        [TypeFilter(typeof(OperationFilter),
            Arguments = new object[] { "Place.Create" })]
        public async Task<IActionResult> PlaceCreate(PlaceEditDto place)
        {
            if (place.Id > 0)
            {
                return Json(new ResultEntity { Message = "请求数据有误，新增数据非0主键" });
            }
            return await CreateOrUpdate(place);
        }

        [HttpPost]
        [TypeFilter(typeof(OperationFilter),
            Arguments = new object[] { "Place.Edit" })]
        public async Task<IActionResult> PlaceEdit(PlaceEditDto place)
        {
            if (place.Id <= 0)
            {
                return Json(new ResultEntity { Message = "请求数据有误，修改数据空主键" });
            }
            return await CreateOrUpdate(place);
        }

        private async Task<IActionResult> CreateOrUpdate(PlaceEditDto place)
        {
            var result = new ResultEntity();
            if (ModelState.IsValid)
            {
                place.SetCreateOrUpdateInfo(CurrentUserId);
                result = await _placeAppService.AddOrUpdateAsync(place);
                return Json(result);
            }
            result.Message = ModelState.Values
                .FirstOrDefault(a => a.ValidationState == ModelValidationState.Invalid)?.Errors.FirstOrDefault()
                ?.ErrorMessage;

            return Json(result);
        }

        public async Task<IActionResult> GetDepartmentsForSelect()
        {
            var departments = await _departmentAppService.GetAllAsync();
            var json = departments.Select(a => new SelectViewModel
            {
                Id = a.Id,
                Text = a.FullName
            });
            return Json(json);
        }
        
        public async Task<IActionResult> GetPlacesForSelect()
        {
            var places = await _placeAppService.GetAllAsync();
            places.Insert(0, new PlaceListDto
            {
                Id = 0,
                Name = "无"
            });
            var json = places.Select(a => new SelectViewModel
            {
                Id = a.Id,
                Text = a.Name
            });
            return Json(json);
        }

        public async Task<IActionResult> GetPlaces(int draw, int start, int length)
        {
            var keywords = Request.Query["search[value]"];
            var pageSearchDto = new PageSearchModel
            {
                Keywords = keywords,
                Take = length,
                Skip = start
            };
            var pagereslut = await _placeAppService.GetPagingAysnc(pageSearchDto);
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
