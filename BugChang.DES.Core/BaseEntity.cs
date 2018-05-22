using System;

namespace BugChang.DES.Core
{

    /// <summary>
    /// 实体基类，所有实体必须继承，主键为int
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public int? CreateBy { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 更新人
        /// </summary>
        public int? UpdateBy { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdateTime { get; set; }


    }

    /// <summary>
    /// 软删除接口
    /// </summary>
    public interface ISoftDelete
    {
        bool IsDeleted { get; set; }
    }
}