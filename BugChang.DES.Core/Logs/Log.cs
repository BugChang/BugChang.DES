using System;
using System.ComponentModel.DataAnnotations.Schema;
using BugChang.DES.Core.Authorization.Users;

namespace BugChang.DES.Core.Logs
{
    public class Log
    {
        public int Id { get; set; }

        public EnumLogType Type { get; set; }

        public EnumLogLevel Level { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public int? OperatorId { get; set; }

        public string Data { get; set; }

        public DateTime CreateTime { get; set; }

        [ForeignKey("OperatorId")]
        public virtual User Operator { get; set; }

    }
}
