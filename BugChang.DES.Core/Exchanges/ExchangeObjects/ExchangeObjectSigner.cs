using BugChang.DES.Core.Commons;

namespace BugChang.DES.Core.Exchanges.ExchangeObjects
{
    public class ExchangeObjectSigner : BaseEntity<int>
    {
        public int ExchangeObjectId { get; set; }

        public int UserId { get; set; }


    }
}
