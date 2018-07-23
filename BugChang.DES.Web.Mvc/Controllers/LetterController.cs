using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Application.Departments;
using BugChang.DES.Application.Groups;
using BugChang.DES.Application.Letters;
using BugChang.DES.Application.Letters.Dtos;
using BugChang.DES.Web.Mvc.Filters;
using BugChang.DES.Web.Mvc.Models.Common;
using Microsoft.AspNetCore.Mvc;
namespace BugChang.DES.Web.Mvc.Controllers
{
    public class LetterController : BaseController
    {
        private readonly ILetterAppService _letterAppService;
        private readonly IDepartmentAppService _departmentAppService;
        private readonly IGroupAppService _groupAppService;
        public LetterController(ILetterAppService letterAppService, IDepartmentAppService departmentAppService, IGroupAppService groupAppService)
        {
            _letterAppService = letterAppService;
            _departmentAppService = departmentAppService;
            _groupAppService = groupAppService;
        }

        [TypeFilter(typeof(MenuFilter))]
        public IActionResult Receive()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Receive(ReceiveLetterEditDto receiveLetter)
        {
            var result = await _letterAppService.AddReceiveLetter(receiveLetter);
            return Json(result);
        }

        public IActionResult Send()
        {
            return View();
        }

        public async Task<IActionResult> GetReceiveGroupSelect()
        {
            var groups = await _groupAppService.GetReceiveLetterGroups();
            var json = groups.Select(a => new SimpleTreeViewModel
            {
                Id = a.Id,
                Name = a.Name
            });
            return Json(json);
        }

        public async Task<IActionResult> GetReceiveDetailSelect(int id)
        {
            var departments = await _departmentAppService.GetAllAsync();
            var groupDetails = await _groupAppService.GetGroupDetails(id);
            departments = departments.Where(a => groupDetails.Select(b => b.DepartmentId).Contains(a.Id)).ToList();
            var json = departments.Select(a => new SimpleTreeViewModel
            {
                Id = a.Id,
                ParentId = a.ParentId,
                Name = a.Name
            });
            return Json(json);
        }
    }
}
