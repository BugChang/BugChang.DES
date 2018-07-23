using BugChang.DES.Core.Commons;

namespace BugChang.DES.Core.SerialNumbers
{
    public class SerialNumber : BaseEntity<int>, ISoftDelete
    {
        /// <summary>
        /// 年度
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public EnumSerialNumberType Type { get; set; }

        /// <summary>
        /// 机构
        /// </summary>
        public int? DepartmentId { get; set; }


        public bool IsDeleted { get; set; }
    }
}
