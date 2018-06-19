using System;

namespace BugChang.DES.Core.Logs
{
    public class Log
    {
        public int Id { get; set; }

        public EnumLogLevel LogLevel { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }
        

        public string Data { get; set; }

        public DateTime CreateTime { get; set; }

    }
}
