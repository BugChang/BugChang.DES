using System;
using BugChang.DES.Core.Commons;

namespace BugChang.DES.Core.Exchanges.Bill
{
    public class ExchangeList : BaseEntity
    {
        public int? ObjectId { get; set; }

        public string ListNo { get; set; }

        public EnumListType Type { get; set; }

        public int DepartmentId { get; set; }

        public bool Printed { get; set; }

        public int ExchangeUserId { get; set; }

        public string ExchangeUserName { get; set; }

        public string ObjectName { get; set; }

        public string GetListNo(int serialNo)
        {
            return DateTime.Now.ToString("yyyy") + serialNo.ToString("00000000");
        }

    }
}
