using System;
using System.Collections.Generic;
using BugChang.DES.Web.Mvc.Filters;
using BugChang.DES.Web.Mvc.Models.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BugChang.DES.Web.Mvc.Controllers
{
    [AutoValidateAntiforgeryToken]
    [Authorize]
    public class BaseController : Controller
    {
        protected CurrentUserModel CurrentUser = new CurrentUserModel();
        //private readonly IOptions<AccountSettings> _accountSettings;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var currentUrl = "/" + context.RouteData.Values["Controller"] + "/" + context.RouteData.Values["Action"];
            ViewBag.Url = currentUrl;
            if (HttpContext.User.FindFirst("Id") != null)
            {
                CurrentUser.UserId = Convert.ToInt32(HttpContext.User.FindFirst("Id").Value);
                CurrentUser.DepartmentId = Convert.ToInt32(HttpContext.User.FindFirst("DepartmentId").Value);
                CurrentUser.UserName = Convert.ToString(HttpContext.User.FindFirst("UserName").Value);
                CurrentUser.DisplayName = Convert.ToString(HttpContext.User.FindFirst("DisplayName").Value);
                CurrentUser.NeedChangePassword = Convert.ToInt32(HttpContext.User.FindFirst("NeedChangePassword").Value) > 0;
                CurrentUser.UsbKeyNo = Convert.ToString(HttpContext.User.FindFirst("UsbKeyNo"));
                //if (_accountSettings.Value.ValidateUsbKeyNo)
                //{
                //    var usbKeyNo = Request.Cookies["KOAL_CERT_CN"].Trim();
                //    if (usbKeyNo != CurrentUser.UsbKeyNo)
                //    {
                //        context.Result = new RedirectToActionResult("UsbKeyNotMacthed", "Account", new { });
                //    }
                //}
                if (CurrentUser.NeedChangePassword && !ForceChangePasswordWhiteList().Contains(currentUrl))
                {
                    context.Result = new RedirectToActionResult("ForceChangePassword", "Account", new { });
                }

            }
        }

        /// <summary>
        /// 强制修改密码的白名单Url
        /// </summary>
        /// <returns></returns>
        private IList<string> ForceChangePasswordWhiteList()
        {
            return new List<string>
            {
                "/Account/ChangePassword",
                "/Account/ForceChangePassword",
                "/Account/Logout"
            };
        }
    }
}
