using BugChang.DES.Application.Commons;
using BugChang.DES.Core.Exchanges.Bill;

namespace BugChang.DES.Application.Bills.Dtos
{
    public class BillDto : BaseDto
    {
        public int? ObjectId { get; set; }

        public string ObjectName { get; set; }

        public string ListNo { get; set; }

        public EnumListType Type { get; set; }

        public int DepartmentId { get; set; }

        public int ExchangeUserId { get; set; }

        public string ExchangeUserName { get; set; }
    }
}
