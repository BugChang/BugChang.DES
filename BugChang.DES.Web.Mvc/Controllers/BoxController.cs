using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Application.Boxs;
using BugChang.DES.Application.Boxs.Dtos;
using BugChang.DES.Application.ExchangeObjects;
using BugChang.DES.Application.Places;
using BugChang.DES.Core.Commons;
using BugChang.DES.Web.Mvc.Filters;
using BugChang.DES.Web.Mvc.Models.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BugChang.DES.Web.Mvc.Controllers
{
    public class BoxController : BaseController
    {
        private readonly IPlaceAppService _placeAppService;
        private readonly IBoxAppService _boxAppService;
        private readonly IExchangeObjectAppService _exchangeObjectAppService;


        public BoxController(IPlaceAppService placeAppService, IBoxAppService boxAppService, IExchangeObjectAppService exchangeObjectAppService)
        {
            _placeAppService = placeAppService;
            _boxAppService = boxAppService;
            _exchangeObjectAppService = exchangeObjectAppService;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return Index();
        }

        public async Task<IActionResult> GetPlaces()
        {

            var places = await _placeAppService.GetAllAsync();
            var json = places.Select(a => new SelectViewModel
            {
                Id = a.Id,
                Text = a.Name
            });
            return Json(json);
        }

        public async Task<IActionResult> GetBoxs(int draw, int start, int length)
        {
            var keywords = Request.Query["search[value]"];
            var pageSearchDto = new PageSearchModel
            {
                Keywords = keywords,
                Take = length,
                Skip = start
            };
            var pagereslut = await _boxAppService.GetPagingAysnc(pageSearchDto);
            var json = new
            {
                draw,
                recordsTotal = pagereslut.Total,
                recordsFiltered = pagereslut.Total,
                data = pagereslut.Rows
            };
            return Json(json);
        }

        public async Task<IActionResult> GetExchangeObjects()
        {
            var exchangeObjects = await _exchangeObjectAppService.GetAlListAsync();
            var json = exchangeObjects.Select(
                a => new SelectViewModel
                {
                    Id = a.Id,
                    Text = a.Name
                });
            return Json(json);
        }

        [HttpPost]
        [TypeFilter(typeof(OperationFilter),
            Arguments = new object[] { "Box.Create" })]
        public async Task<IActionResult> Create(BoxEditDto box)
        {
            if (box.Id > 0)
            {
                return Json(new ResultEntity { Message = "请求数据有误，新增数据非0主键" });
            }
            return await CreateOrUpdate(box);
        }

        [HttpPost]
        [TypeFilter(typeof(OperationFilter),
            Arguments = new object[] { "Box.Edit" })]
        public async Task<IActionResult> Edit(BoxEditDto box)
        {
            if (box.Id <= 0)
            {
                return Json(new ResultEntity { Message = "请求数据有误，修改数据空主键" });
            }
            return await CreateOrUpdate(box);
        }

        private async Task<IActionResult> CreateOrUpdate(BoxEditDto box)
        {
            var result = new ResultEntity();
            if (ModelState.IsValid)
            {
                if (box.PlaceId > 0)
                {
                    box.SetCreateOrUpdateInfo(CurrentUserId);
                    result = await _boxAppService.AddOrUpdateAsync(box);
                    return Json(result);
                }

                result.Message = "交换场所不能为空";
                return Json(result);
            }
            result.Message = ModelState.Values
                .FirstOrDefault(a => a.ValidationState == ModelValidationState.Invalid)?.Errors.FirstOrDefault()
                ?.ErrorMessage;

            return Json(result);
        }

        public async Task<IActionResult> EditBoxModal(int id)
        {
            var box = await _boxAppService.GetForEditByIdAsync(id);
            return PartialView("_EditBoxModal", box);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var result = await _boxAppService.DeleteByIdAsync(id, CurrentUserId);
            return Json(result);
        }

        public IActionResult AssignObjectModal(int id)
        {
            ViewBag.BoxId = id;
            return PartialView("_AssignObjectModal");
        }

        public async Task<IActionResult> GetBoxObjects(int id)
        {
            var objects = await _exchangeObjectAppService.GetAlListAsync();
            var boxObjectIds = await _boxAppService.GetBoxObjectIds(id);
            var json = objects.Select(a => new SelectViewModel
            {
                Id = a.Id,
                Text = a.Name,
                Selected = boxObjectIds.Any(b => b == a.Id)
            });
            return Json(json);
        }

        [HttpPost]
        [TypeFilter(typeof(OperationFilter),
            Arguments = new object[] { "Box.AssignObject" })]
        public async Task<IActionResult> AssignObject(int boxId, List<int> objectIds)
        {
            var result = await _boxAppService.AssignObject(boxId, objectIds, CurrentUserId);
            return Json(result);
        }
    }
}
