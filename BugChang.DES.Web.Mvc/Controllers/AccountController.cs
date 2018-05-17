using System;
using System.Collections.Generic;
using BugChang.DES.Web.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using BugChang.DES.Application.UserApp;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace BugChang.DES.Web.Mvc.Controllers
{
    public class AccountController : BaseController
    {

        private readonly IUserAppService _userAppService;
        public AccountController(IUserAppService userAppService)
        {
            _userAppService = userAppService;
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
                var result = await _userAppService.CheckAsync(model.UserName, model.Password);
                if (result)
                {
                    var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, model.UserName),
                };

                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);


                    await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity),
                            new AuthenticationProperties
                            {
                                IsPersistent = model.RememberMe,
                                ExpiresUtc = DateTimeOffset.Now.AddMinutes(20)
                            });

                    return Redirect(returnUrl);
                }
                else
                {
                    ViewBag.ErrorMessage = "用户名或密码错误";
                }
            }

            return View();
        }
    }
}
