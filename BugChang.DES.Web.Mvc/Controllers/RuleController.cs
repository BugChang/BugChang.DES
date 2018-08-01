using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Application.Barcodes;
using BugChang.DES.Application.Rules;
using BugChang.DES.Application.Rules.Dtos;
using BugChang.DES.Core.Commons;
using BugChang.DES.Web.Mvc.Filters;
using BugChang.DES.Web.Mvc.Models.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BugChang.DES.Web.Mvc.Controllers
{
    public class RuleController : BaseController
    {
        private readonly IRuleAppService _ruleAppService;
        private readonly IBarcodeAppService _barcodeAppService;
        public RuleController(IRuleAppService ruleAppService, IBarcodeAppService barcodeAppService)
        {
            _ruleAppService = ruleAppService;
            _barcodeAppService = barcodeAppService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> EditRuleModal(int id)
        {
            var barcodeRule = await _ruleAppService.GetForEditByIdAsync(id);
            return PartialView("_EditRuleModal", barcodeRule);
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
            Arguments = new object[] { "Rule.Create" })]
        public async Task<IActionResult> Create(RuleEditDto barcodeRule)
        {
            if (barcodeRule.Id > 0)
            {
                return Json(new ResultEntity { Message = "请求数据有误，新增数据非0主键" });
            }
            return await CreateOrUpdate(barcodeRule);
        }

        [HttpPost]
        [TypeFilter(typeof(OperationFilter),
            Arguments = new object[] { "Rule.Edit" })]
        public async Task<IActionResult> Edit(RuleEditDto barcodeRule)
        {
            if (barcodeRule.Id <= 0)
            {
                return Json(new ResultEntity { Message = "请求数据有误，修改数据空主键" });
            }
            return await CreateOrUpdate(barcodeRule);
        }

        private async Task<IActionResult> CreateOrUpdate(RuleEditDto barcodeRule)
        {
            var result = new ResultEntity();
            if (ModelState.IsValid)
            {

                barcodeRule.SetCreateOrUpdateInfo(CurrentUser.UserId);
                result = await _ruleAppService.AddOrUpdateAsync(barcodeRule);
                return Json(result);
            }
            result.Message = ModelState.Values
                .FirstOrDefault(a => a.ValidationState == ModelValidationState.Invalid)?.Errors.FirstOrDefault()
                ?.ErrorMessage;

            return Json(result);
        }

        public async Task<IActionResult> GetRules(int draw, int start, int length)
        {
            var keywords = Request.Query["search[value]"];
            var pageSearchDto = new PageSearchCommonModel
            {
                Keywords = keywords,
                Take = length,
                Skip = start
            };
            var pagereslut = await _ruleAppService.GetPagingAysnc(pageSearchDto);
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
        [TypeFilter(typeof(OperationFilter),
            Arguments = new object[] { "Rule.Delete" })]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _ruleAppService.DeleteByIdAsync(id, CurrentUser.UserId);
            return Json(result);
        }
    }
}
