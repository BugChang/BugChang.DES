using BugChang.DES.Core.Commons;

namespace BugChang.DES.Core.Authentication.Card
{
    public class Card : BaseEntity, ISoftDelete
    {

        public int UserId { get; set; }

        public string Number { get; set; }

        public string Value { get; set; }

        public bool IsDeleted { get; set; }
    }
}
