using BugChang.DES.Core.Commons;

namespace BugChang.DES.Core.Exchanges.ExchangeObjects
{
    public class ExchangeObject : BaseEntity, ISoftDelete
    {
        public string Name { get; set; }

        public EnumObjectType ObjectType { get; set; }

        public int Value { get; set; }

        public bool IsDeleted { get; set; }
    }
}
