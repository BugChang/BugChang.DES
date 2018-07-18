using System;
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
        protected int CurrentUserId;
        protected int CurrentDepartmentId;
        public BaseController()
        {
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (HttpContext.User.FindFirst("Id") != null)
            {
                CurrentUserId = Convert.ToInt32(HttpContext.User.FindFirst("Id").Value);
                CurrentDepartmentId = Convert.ToInt32(HttpContext.User.FindFirst("DepartmentId").Value);
            }
            ViewBag.Url = "/" + context.RouteData.Values["Controller"] + "/" + context.RouteData.Values["Action"];
        }
    }
}
