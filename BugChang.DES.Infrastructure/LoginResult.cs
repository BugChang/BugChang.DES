using System.Security.Claims;

namespace BugChang.DES.Infrastructure
{
    public class LoginResult
    {
        public LoginResult()
        {
            Result = EnumLoginResult.用户名或密码错误;
        }

        public EnumLoginResult Result { get; set; }

        public ClaimsPrincipal ClaimsPrincipal { get; set; }
    }

    public enum EnumLoginResult
    {
        登陆成功,
        用户名或密码错误,
        账号已锁定,
        账号已停用
    }
}
