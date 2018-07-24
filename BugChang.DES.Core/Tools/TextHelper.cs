namespace BugChang.DES.Core.Tools
{
    public static class TextHelper
    {
        /// <summary>
        /// 字符串后面补零
        /// </summary>
        /// <param name="text">原始文本</param>
        /// <param name="limitedLength">规定长度</param>
        /// <returns></returns>
        public static string RepairZeroRight(string text, int limitedLength)
        {
            //补足0的字符串
            string temp = "";
            //补足0
            for (int i = 0; i < limitedLength - text.Length; i++)
            {
                temp += "0";
            }
            //连接text
            text += temp;
            //返回补足0的字符串
            return text;
        }
    }
}
