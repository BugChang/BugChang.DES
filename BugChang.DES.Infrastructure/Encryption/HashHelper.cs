using NETCore.Encrypt;

namespace BugChang.DES.Infrastructure.Encryption
{
    public static class HashHelper
    {
        public static string Md5(string str)
        {
            return EncryptProvider.Md5(str);
        }

        public static string Sha1(string str)
        {
            return EncryptProvider.Sha1(str);
        }


        public static string Sha256(string str)
        {
            return EncryptProvider.Sha256(str);
        }

        public static string Sha384(string str)
        {
            return EncryptProvider.Sha384(str);
        }

        public static string Sha512(string str)
        {
            return EncryptProvider.Sha512(str);
        }
    }
}
