using System;
using System.ComponentModel.DataAnnotations.Schema;
using BugChang.DES.Core.Authorization.Users;

namespace BugChang.DES.Core.Commons
{


    public abstract class BaseEntity<TPrimaryKey>
    {
        public TPrimaryKey Id { get; set; }
    }


    /// <summary>
    /// 实体基类，所有实体必须继承，主键为int
    /// </summary>
    public abstract class BaseEntity : BaseEntity<int>
    {
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

        [ForeignKey("CreateBy")]
        public virtual User CreateUser { get; set; }


        [ForeignKey("UpdateBy")]
        public virtual User UpdateUser { get; set; }

        public void SetUpdateInfo()
        {;
            UpdateTime = DateTime.Now;
        }

        public void SetCreateInfo()
        {
            CreateTime = DateTime.Now;
        }
    }


    /// <summary>
    /// 软删除接口
    /// </summary>
    public interface ISoftDelete
    {
        /// <summary>
        /// 删除标识
        /// </summary>
        bool IsDeleted { get; set; }
    }
}