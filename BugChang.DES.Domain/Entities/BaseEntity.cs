namespace BugChang.DES.Domain.Entities
{

    /// <summary>
    /// 实体基类，所有实体必须继承，主键为int
    /// </summary>
    public abstract class BaseEntity
    {
        public int Id { get; set; }
    }
}