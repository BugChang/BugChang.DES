using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using BugChang.DES.Application.Accounts;
using BugChang.DES.Core.Authentication;
using BugChang.DES.Core.Security;
using BugChang.DES.Web.Mvc.Models.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;

namespace BugChang.DES.Web.Mvc.Controllers
{
    public class AccountController : BaseController
    {

        private readonly IAccountAppService _accountAppService;
        private readonly IOptions<LoginSettings> _loginSettings;
        public AccountController(IAccountAppService accountAppService, IOptions<LoginSettings> loginSettings)
        {
            _accountAppService = accountAppService;
            _loginSettings = loginSettings;
        }

        [AllowAnonymous]
        public IActionResult Login(string returnUrl = "")
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginViewModel());
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="model"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = "")
        {
            if (ModelState.IsValid)
            {

                var loginResult = await _accountAppService.LoginAsync(model.UserName, HashHelper.Md5(model.Password));

                switch (loginResult.Result)
                {
                    case EnumLoginResult.登录成功:
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, loginResult.ClaimsPrincipal, new AuthenticationProperties
                        {
                            IsPersistent = model.RememberMe,
                            ExpiresUtc = DateTimeOffset.Now.AddMinutes(_loginSettings.Value.ExpiryTime)
                        });
                        returnUrl = string.IsNullOrEmpty(returnUrl) ? "/Home/Index" : returnUrl;
                        return Redirect(returnUrl);
                    default:
                        ViewBag.ErrorMessage = loginResult.ToString();
                        break;
                }
            }
            ViewBag.ErrorMessage = ViewBag.ErrorMessage ?? ModelState.Values
                .FirstOrDefault(a => a.ValidationState == ModelValidationState.Invalid)
                ?.Errors.FirstOrDefault()
                ?.ErrorMessage;

            return View(model);
        }


        /// <summary>
        ///   登出
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}
