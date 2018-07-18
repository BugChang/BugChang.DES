using BugChang.DES.Core.Commons;

namespace BugChang.DES.Core.Exchanges.Bill
{
    public class ExchangeList : BaseEntity
    {
        public int ObjectId { get; set; }

        public EnumListType Type { get; set; }

        public bool Printed { get; set; }

    }
}
