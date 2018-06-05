using BugChang.DES.Web.Mvc.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BugChang.DES.Web.Mvc.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
    }
}
