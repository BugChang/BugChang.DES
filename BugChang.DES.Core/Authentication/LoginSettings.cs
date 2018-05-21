namespace BugChang.DES.Core.Authentication
{
    public class LoginSettings
    {

        /// <summary>
        /// 登录错误锁定的次数
        /// </summary>
        public int LoginErrorCount2Lock { get; set; }

        /// <summary>
        /// 密码最小位数
        /// </summary>
        public int PasswordMinLength { get; set; }

    }
}
