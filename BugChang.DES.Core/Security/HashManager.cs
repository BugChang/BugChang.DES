using NETCore.Encrypt;

namespace BugChang.DES.Core.Security
{
    /// <summary>
    /// HASH加密操作帮助类
    /// </summary>
    public static class HashHelper
    {
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="str">未加密字符串</param>
        /// <returns>已加密字符串</returns>
        public static string Md5(string str)
        {
            return EncryptProvider.Md5(str);
        }

        /// <summary>
        /// SHA1加密
        /// </summary>
        /// <param name="str">未加密字符串</param>
        /// <returns>已加密字符串</returns>
        public static string Sha1(string str)
        {
            return EncryptProvider.Sha1(str);
        }


        /// <summary>
        /// SHA256加密
        /// </summary>
        /// <param name="str">未加密字符串</param>
        /// <returns>已加密字符串</returns>
        public static string Sha256(string str)
        {
            return EncryptProvider.Sha256(str);
        }

        /// <summary>
        /// SHA384加密
        /// </summary>
        /// <param name="str">未加密字符串</param>
        /// <returns>已加密字符串</returns>
        public static string Sha384(string str)
        {
            return EncryptProvider.Sha384(str);
        }

        /// <summary>
        /// SHA512加密
        /// </summary>
        /// <param name="str">未加密字符串</param>
        /// <returns>已加密字符串</returns>
        public static string Sha512(string str)
        {
            return EncryptProvider.Sha512(str);
        }
    }
}
