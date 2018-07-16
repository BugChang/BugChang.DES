using System.ComponentModel.DataAnnotations.Schema;
using BugChang.DES.Core.Authorization.Users;
using BugChang.DES.Core.Commons;

namespace BugChang.DES.Core.Exchanges.ExchangeObjects
{
    public class ExchangeObjectSigner : BaseEntity<int>
    {
        public int ExchangeObjectId { get; set; }

        public int UserId { get; set; }

        [ForeignKey("ExchangeObjectId")]
        public virtual ExchangeObject ExchangeObject { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

    }
}
