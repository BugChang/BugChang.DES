using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using BugChang.DES.Application.Accounts;
using BugChang.DES.Application.Cards;
using BugChang.DES.Application.Clients;
using BugChang.DES.Application.Users;
using BugChang.DES.Core.Authentication;
using BugChang.DES.Core.Commons;
using BugChang.DES.Core.Tools;
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
        private readonly IOptions<AccountSettings> _loginSettings;
        private readonly IClientAppService _clientAppService;
        private readonly ICardAppService _cardAppService;
        private readonly IUserAppService _userAppService;
        public AccountController(IAccountAppService accountAppService, IOptions<AccountSettings> loginSettings, IClientAppService clientAppService, ICardAppService cardAppService, IUserAppService userAppService)
        {
            _accountAppService = accountAppService;
            _loginSettings = loginSettings;
            _clientAppService = clientAppService;
            _cardAppService = cardAppService;
            _userAppService = userAppService;
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
                var usbKeyNo = Request.Cookies["KOAL_CERT_CN"].Trim();
                var loginResult = await _accountAppService.LoginAsync(model.UserName, HashHelper.Md5(model.Password), usbKeyNo);

                switch (loginResult.Result)
                {
                    case EnumLoginResult.登录成功:
                    case EnumLoginResult.强制修改密码:
                        var client = await _clientAppService.GetClient(model.DeviceCode);
                        if (client != null)
                        {
                            returnUrl = string.IsNullOrWhiteSpace(returnUrl) ? client.HomePage : returnUrl;
                        }
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

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> LoginWithCard(string deviceCode, string cardNo)
        {
            var result = new ResultEntity();
            var card = await _cardAppService.GetCardByNo(cardNo);
            if (!card.Enabled)
            {
                result.Message = "该卡片尚未启用，无法登录";
            }
            else
            {
                var user = await _userAppService.GetForEditByIdAsync(card.UserId);
                var usbKeyNo = Request.Cookies["KOAL_CERT_CN"].Trim();
                var loginResult = await _accountAppService.LoginAsync(user.UserName, user.Password, usbKeyNo);
                switch (loginResult.Result)
                {
                    case EnumLoginResult.登录成功:
                    case EnumLoginResult.强制修改密码:
                        var client = await _clientAppService.GetClient(deviceCode);
                        if (client != null)
                        {
                            result.Data = client.HomePage;
                        }
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, loginResult.ClaimsPrincipal, new AuthenticationProperties
                        {
                            ExpiresUtc = DateTimeOffset.Now.AddMinutes(_loginSettings.Value.ExpiryTime)
                        });
                        result.Data = string.IsNullOrEmpty(result.Data) ? "/Home/Index" : result.Data;
                        break;
                    default:
                        result.Message = result.Message;
                        break;
                }
            }

            return Json(result);
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

        public IActionResult ChangePassword()
        {
            var model = new ChangePasswordViewModel
            {
                Id = CurrentUser.UserId,
                UserName = CurrentUser.UserName
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel changePassword)
        {
            var result = new ResultEntity();
            ViewBag.ChangePasswordFlag = 0;
            if (ModelState.IsValid)
            {
                result = await _accountAppService.ChangePassword(changePassword.Id, changePassword.Password, changePassword.OldPassword);
            }
            else
            {
                result.Message = ModelState.Values.FirstOrDefault(a => a.ValidationState == ModelValidationState.Invalid)?.Errors.FirstOrDefault()?.ErrorMessage;
            }

            return Json(result);
        }

        public IActionResult ForceChangePassword()
        {
            return View();
        }

        public IActionResult UsbKeyNotMacthed()
        {
            return View();
        }
    }
}
