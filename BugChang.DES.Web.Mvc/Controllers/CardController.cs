using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Application.Cards;
using BugChang.DES.Application.Cards.Dtos;
using BugChang.DES.Application.Users;
using BugChang.DES.Core.Commons;
using BugChang.DES.Web.Mvc.Filters;
using BugChang.DES.Web.Mvc.Models.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BugChang.DES.Web.Mvc.Controllers
{
    [ServiceFilter(typeof(RefererFilter))]
    public class CardController : BaseController
    {
        private readonly ICardAppService _cardAppService;
        private readonly IUserAppService _userAppService;

        public CardController(ICardAppService cardAppService, IUserAppService userAppService)
        {
            _cardAppService = cardAppService;
            _userAppService = userAppService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> EditCardModal(int id)
        {
            var model = await _cardAppService.GetForEditByIdAsync(id);
            return PartialView("_EditCardModal", model);
        }

        [HttpPost]
        [TypeFilter(typeof(OperationFilter),
            Arguments = new object[] { "Card.Create" })]
        public async Task<IActionResult> Create(CardEditDto card)
        {
            if (card.Id > 0)
            {
                return Json(new ResultEntity { Message = "请求数据有误，新增数据非0主键" });
            }
            return await CreateOrUpdate(card);
        }

        [HttpPost]
        [TypeFilter(typeof(OperationFilter),
            Arguments = new object[] { "Card.Edit" })]
        public async Task<IActionResult> Edit(CardEditDto card)
        {
            if (card.Id <= 0)
            {
                return Json(new ResultEntity { Message = "请求数据有误，修改数据空主键" });
            }
            return await CreateOrUpdate(card);
        }

        private async Task<IActionResult> CreateOrUpdate(CardEditDto card)
        {
            var result = new ResultEntity();
            if (ModelState.IsValid)
            {

                card.SetCreateOrUpdateInfo(CurrentUser.UserId);
                result = await _cardAppService.AddOrUpdateAsync(card);
                return Json(result);
            }
            result.Message = ModelState.Values
                .FirstOrDefault(a => a.ValidationState == ModelValidationState.Invalid)?.Errors.FirstOrDefault()
                ?.ErrorMessage;

            return Json(result);
        }

        public async Task<IActionResult> GetCards(int draw, int start, int length)
        {
            var keywords = Request.Query["search[value]"];
            var pageSearchDto = new PageSearchCommonModel
            {
                Keywords = keywords,
                Take = length,
                Skip = start,
                UserId = CurrentUser.UserId
            };
            var pagereslut = await _cardAppService.GetPagingAysnc(pageSearchDto);
            var json = new
            {
                draw,
                recordsTotal = pagereslut.Total,
                recordsFiltered = pagereslut.Total,
                data = pagereslut.Rows
            };
            return Json(json);
        }

        public async Task<IActionResult> GetUsers()
        {
            var users = await _userAppService.GetUsersAsync();
            var json = users.Select(a => new SelectViewModel
            {
                Id = a.Id,
                Text = a.DisplayName
            });
            return Json(json);
        }

        [HttpPost]
        [TypeFilter(typeof(OperationFilter),
            Arguments = new object[] { "Card.Delete" })]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _cardAppService.DeleteByIdAsync(id, CurrentUser.UserId);
            return Json(result);
        }

        [HttpPost]
        [TypeFilter(typeof(OperationFilter),
            Arguments = new object[] { "Card.Enabled" })]
        public async Task<IActionResult> ChangeEnabled(int id)
        {
            var result = await _cardAppService.ChangeEnabled(id, CurrentUser.UserId);
            return Json(result);
        }
    }
}
