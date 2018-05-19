using System;
using System.Collections.Generic;
using System.Linq;
using BugChang.DES.Web.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using BugChang.DES.Application.Accounts;
using BugChang.DES.Application.UserApp;
using BugChang.DES.Infrastructure;
using BugChang.DES.Infrastructure.Encryption;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BugChang.DES.Web.Mvc.Controllers
{
    public class AccountController : BaseController
    {

        private readonly IAccountAppService _accountAppService;
        public AccountController(IAccountAppService accountAppService)
        {
            _accountAppService = accountAppService;
        }

        [AllowAnonymous]
        public IActionResult Login(string returnUrl = "/")
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginViewModel());
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = "")
        {
            if (ModelState.IsValid)
            {

                var loginResult = await _accountAppService.LoginAsync(model.UserName, HashHelper.Md5(model.Password));

                switch (loginResult.Result)
                {
                    case EnumLoginResult.登陆成功:
                        await HttpContext.SignInAsync(loginResult.ClaimsPrincipal, new AuthenticationProperties()
                        {
                            IsPersistent = model.RememberMe,
                            ExpiresUtc = DateTimeOffset.Now.AddMinutes(20)
                        });

                        return Redirect(returnUrl);
                    default:
                        ViewBag.ErrorMessage = loginResult.Result.ToString();
                        break;
                }
            }
            ViewBag.ErrorMessage = ModelState.Values
                .FirstOrDefault(a => a.ValidationState == ModelValidationState.Invalid)
                ?.Errors.FirstOrDefault()
                ?.ErrorMessage;

            return View(model);
        }
    }
}
