using System.ComponentModel.DataAnnotations.Schema;
using BugChang.DES.Core.Commons;
using BugChang.DES.Core.Exchanges.ExchangeObjects;

namespace BugChang.DES.Core.Exchanges.Boxs
{
    public class BoxObject : BaseEntity<int>
    {
        public int BoxId { get; set; }

        public int ExchangeObjectId { get; set; }

        [ForeignKey("BoxId")]
        public virtual Box Box { get; set; }

        [ForeignKey("ExchangeObjectId")]
        public virtual ExchangeObject ExchangeObject { get; set; }
    }
}
