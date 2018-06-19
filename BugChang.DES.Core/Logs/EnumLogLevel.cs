using System.ComponentModel;

namespace BugChang.DES.Core.Logs
{
    public enum EnumLogLevel
    {

        [Description("调试")]
        Debug,

        [Description("信息")]
        Information,

        [Description("警告")]
        Warnning,

        [Description("错误")]
        Error,


        [Description("致命")]
        Fatal
    }
}
