using BugChang.DES.Application.Menus;
using BugChang.DES.Web.Mvc.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BugChang.DES.Web.Mvc.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
    }
}
