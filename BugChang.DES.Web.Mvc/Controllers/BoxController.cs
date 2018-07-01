using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Application.Boxs;
using BugChang.DES.Application.Places;
using BugChang.DES.Core.Commons;
using BugChang.DES.Web.Mvc.Models.Common;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BugChang.DES.Web.Mvc.Controllers
{
    public class BoxController : BaseController
    {
        private readonly IPlaceAppService _placeAppService;
        private readonly IBoxAppService _boxAppService;

        public BoxController(IPlaceAppService placeAppService, IBoxAppService boxAppService)
        {
            _placeAppService = placeAppService;
            _boxAppService = boxAppService;
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
    }
}
