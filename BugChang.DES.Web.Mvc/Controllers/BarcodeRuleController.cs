using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Application.BarcodeRules;
using BugChang.DES.Application.BarcodeRules.Dtos;
using BugChang.DES.Application.Barcodes;
using BugChang.DES.Application.Boxs.Dtos;
using BugChang.DES.Application.Channels;
using BugChang.DES.Core.Commons;
using BugChang.DES.Web.Mvc.Filters;
using BugChang.DES.Web.Mvc.Models.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BugChang.DES.Web.Mvc.Controllers
{
    public class BarcodeRuleController : BaseController
    {
        private readonly IBarcodeRuleAppService _barcodeRuleAppService;
        private readonly IBarcodeAppService _barcodeAppService;
        public BarcodeRuleController(IBarcodeRuleAppService barcodeRuleAppService, IBarcodeAppService barcodeAppService)
        {
            _barcodeRuleAppService = barcodeRuleAppService;
            _barcodeAppService = barcodeAppService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> EditBarcodeRuleModal(int id)
        {
            var barcodeRule = await _barcodeRuleAppService.GetForEditByIdAsync(id);
            return PartialView("_EditBarcodeRuleModal", barcodeRule);
        }

        public IActionResult GetBarcodeTypes()
        {
            var barcodeTypes = _barcodeAppService.GetBarcodeTypes();
            var json = barcodeTypes.Select(a => new SelectViewModel
            {
                Id = a.Id,
                Text = a.Name
            });
            return Json(json);
        }

        [HttpPost]
        [TypeFilter(typeof(OperationFilter),
            Arguments = new object[] { "BarcodeRule.Create" })]
        public async Task<IActionResult> Create(BarcodeRuleEditDto barcodeRule)
        {
            if (barcodeRule.Id > 0)
            {
                return Json(new ResultEntity { Message = "请求数据有误，新增数据非0主键" });
            }
            return await CreateOrUpdate(barcodeRule);
        }

        [HttpPost]
        [TypeFilter(typeof(OperationFilter),
            Arguments = new object[] { "BarcodeRule.Edit" })]
        public async Task<IActionResult> Edit(BarcodeRuleEditDto barcodeRule)
        {
            if (barcodeRule.Id <= 0)
            {
                return Json(new ResultEntity { Message = "请求数据有误，修改数据空主键" });
            }
            return await CreateOrUpdate(barcodeRule);
        }

        private async Task<IActionResult> CreateOrUpdate(BarcodeRuleEditDto barcodeRule)
        {
            var result = new ResultEntity();
            if (ModelState.IsValid)
            {

                barcodeRule.SetCreateOrUpdateInfo(CurrentUserId);
                result = await _barcodeRuleAppService.AddOrUpdateAsync(barcodeRule);
                return Json(result);
            }
            result.Message = ModelState.Values
                .FirstOrDefault(a => a.ValidationState == ModelValidationState.Invalid)?.Errors.FirstOrDefault()
                ?.ErrorMessage;

            return Json(result);
        }

        public async Task<IActionResult> GetBarcodeRules(int draw, int start, int length)
        {
            var keywords = Request.Query["search[value]"];
            var pageSearchDto = new PageSearchModel
            {
                Keywords = keywords,
                Take = length,
                Skip = start
            };
            var pagereslut = await _barcodeRuleAppService.GetPagingAysnc(pageSearchDto);
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
