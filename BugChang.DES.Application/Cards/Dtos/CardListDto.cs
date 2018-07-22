using BugChang.DES.Application.Commons;

namespace BugChang.DES.Application.Cards.Dtos
{
    public class CardListDto : BaseDto
    {
        public string UserName { get; set; }

        public string Number { get; set; }

        public string Value { get; set; }

        public bool Enabled { get; set; }
    }
}
