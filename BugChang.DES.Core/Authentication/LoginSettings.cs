namespace BugChang.DES.Core.Authentication
{
    public class AccountSettings
    {

        /// <summary>
        /// 登录错误锁定的次数
        /// </summary>
        public int LoginErrorCount2Lock { get; set; }

        /// <summary>
        /// 密码最小位数
        /// </summary>
        public int PasswordMinLength { get; set; }

        /// <summary>
        /// 登录过期时间（分钟）
        /// </summary>
        public int ExpiryTime { get; set; }

        /// <summary>
        /// 强制修改密码
        /// </summary>
        public bool ForceChangePassword { get; set; }

    }
}
