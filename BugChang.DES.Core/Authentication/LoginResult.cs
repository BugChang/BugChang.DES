using System;
using System.Security.Claims;

namespace BugChang.DES.Core.Authentication
{
    public class LoginResult
    {
        public LoginResult()
        {
            Result = EnumLoginResult.用户名或密码错误;
            Message = "";
        }

        /// <summary>
        /// 登录结果
        /// </summary>
        public EnumLoginResult Result { get; set; }

        /// <summary>
        /// 附加信息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// ClaimsPrincipal
        /// </summary>
        public ClaimsPrincipal ClaimsPrincipal { get; set; }

        public override string ToString()
        {
            switch (Result)
            {
                case EnumLoginResult.登录成功:
                    Message = "，正在跳转页面...";
                    break;
                case EnumLoginResult.用户名或密码错误:
                    break;
                case EnumLoginResult.账号已锁定:
                    Message = "，请联系管理员解锁！";
                    break;
                case EnumLoginResult.账号已停用:
                    Message = "，请联系管理员启用！";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            Message = Result + Message;
            return Message;
        }
    }

    public enum EnumLoginResult
    {
        登录成功,
        用户名或密码错误,
        强制修改密码,
        账号已锁定,
        账号已停用
    }
}
