using BugChang.DES.Application.Commons;
using BugChang.DES.Core.Exchanges.Channel;

namespace BugChang.DES.Application.Letters.Dtos
{
    public class SortingListDto : BaseDto
    {
        public string ListNo { get; set; }

        public string Channel { get; set; }

        public int AllCount { get; set; }
    }
}
