using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Application.Clients;
using BugChang.DES.Application.Clients.Dtos;
using BugChang.DES.Application.Places;
using BugChang.DES.Core.Commons;
using BugChang.DES.Web.Mvc.Filters;
using BugChang.DES.Web.Mvc.Models.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BugChang.DES.Web.Mvc.Controllers
{
    [ServiceFilter(typeof(RefererFilter))]
    public class ClientController : BaseController
    {
        private readonly IClientAppService _clientAppService;
        private readonly IPlaceAppService _placeAppService;

        public ClientController(IClientAppService clientAppService, IPlaceAppService placeAppService)
        {
            _clientAppService = clientAppService;
            _placeAppService = placeAppService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> EditClientModal(int id)
        {
            var client = await _clientAppService.GetForEditByIdAsync(id);
            return PartialView("_EditClientModal", client);
        }

        [HttpPost]
        [TypeFilter(typeof(OperationFilter),
            Arguments = new object[] { "Client.Create" })]
        public async Task<IActionResult> Create(ClientEditDto client)
        {
            if (client.Id > 0)
            {
                return Json(new ResultEntity { Message = "请求数据有误，新增数据非0主键" });
            }
            return await CreateOrUpdate(client);
        }

        [HttpPost]
        [TypeFilter(typeof(OperationFilter),
            Arguments = new object[] { "Client.Edit" })]
        public async Task<IActionResult> Edit(ClientEditDto client)
        {
            if (client.Id <= 0)
            {
                return Json(new ResultEntity { Message = "请求数据有误，修改数据空主键" });
            }
            return await CreateOrUpdate(client);
        }

        private async Task<IActionResult> CreateOrUpdate(ClientEditDto client)
        {
            var result = new ResultEntity();
            if (ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(client.HomePage))
                {
                    client.HomePage = "/Home/Index";
                }
                client.SetCreateOrUpdateInfo(CurrentUser.UserId);
                result = await _clientAppService.AddOrUpdateAsync(client);
                return Json(result);
            }
            result.Message = ModelState.Values
                .FirstOrDefault(a => a.ValidationState == ModelValidationState.Invalid)?.Errors.FirstOrDefault()
                ?.ErrorMessage;

            return Json(result);
        }

        public async Task<IActionResult> GetClients(int draw, int start, int length)
        {
            var keywords = Request.Query["search[value]"];
            var pageSearchDto = new PageSearchCommonModel
            {
                Keywords = keywords,
                Take = length,
                Skip = start
            };
            var pagereslut = await _clientAppService.GetPagingAysnc(pageSearchDto);
            var json = new
            {
                draw,
                recordsTotal = pagereslut.Total,
                recordsFiltered = pagereslut.Total,
                data = pagereslut.Rows
            };
            return Json(json);
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

        [HttpPost]
        [TypeFilter(typeof(OperationFilter),
            Arguments = new object[] { "Client.Delete" })]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _clientAppService.DeleteByIdAsync(id, CurrentUser.UserId);
            return Json(result);
        }
    }
}
