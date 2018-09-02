using System;

namespace BugChang.DES.Application.BackUps.Dtos
{
    public class BackUpListDto
    {
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// MD5值
        /// </summary>
        public string Md5 { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        public DateTime DateTime { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        public string OperatorName { get; set; }

        /// <summary>
        /// 类型：1自动，2手动
        /// </summary>
        public int Type { get; set; }
    }
}
