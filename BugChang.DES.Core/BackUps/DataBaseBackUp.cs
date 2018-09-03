using System;
using BugChang.DES.Core.Commons;

namespace BugChang.DES.Core.BackUps
{
    public class DataBaseBackUp : BaseEntity<int>,ISoftDelete
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

        /// <summary>
        /// 备份说明
        /// </summary>
        public string Remark { get; set; }
        public bool IsDeleted { get; set; }
    }
}
