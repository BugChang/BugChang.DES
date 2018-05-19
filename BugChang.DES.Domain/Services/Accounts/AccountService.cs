using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using BugChang.DES.Domain.IRepositories;
using BugChang.DES.Infrastructure;

namespace BugChang.DES.Domain.Services.Accounts
{
    public class AccountService : IAccountServcice
    {
        private readonly IUserRepository _userRepository;

        public AccountService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// 检查用户登录
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        public async Task<LoginResult> LoginAysnc(string userName, string password)
        {
            var loginResult = new LoginResult();
            var user = await _userRepository.GetAsync(userName, password);
            if (user == null)
            {
                loginResult.Result = EnumLoginResult.用户名或密码错误;
            }
            else
            {
                if (!user.Enabled)
                {
                    loginResult.Result = EnumLoginResult.账号已停用;
                }
                else
                {
                    loginResult.Result = EnumLoginResult.登陆成功;
                    var claims = new List<Claim>
                    {
                        new Claim("Id",user.Id.ToString()),
                        new Claim("UserName",user.UserName),
                        new Claim("DisplayName",user.DisplayName)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims);
                    loginResult.ClaimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                }
            }
            return loginResult;
        }

    }
}
