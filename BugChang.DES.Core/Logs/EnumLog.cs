using System.ComponentModel;

namespace BugChang.DES.Core.Logs
{
    public enum EnumLogLevel
    {

        [Description("调试")]
        Debug = 0,

        [Description("信息")]
        Information = 1,

        [Description("警告")]
        Warnning = 2,

        [Description("错误")]
        Error = 3,

        [Description("致命")]
        Fatal = 4
    }

    public enum EnumLogType
    {

        [Description("系统日志")]
        System = 0,

        [Description("审计日志")]
        Audit = 1
    }
}
