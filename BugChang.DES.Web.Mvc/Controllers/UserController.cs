using System.Threading.Tasks;
using BugChang.DES.Application.Users;
using Microsoft.AspNetCore.Mvc;

namespace BugChang.DES.Web.Mvc.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUserAppService _userAppService;

        public UserController(IUserAppService userAppService)
        {
            _userAppService = userAppService;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _userAppService.GetUsersAsync();
            return View(model);
        }
    }
}
