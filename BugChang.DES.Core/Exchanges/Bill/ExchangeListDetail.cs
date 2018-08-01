using System;
using BugChang.DES.Core.Commons;

namespace BugChang.DES.Core.Exchanges.Bill
{
    public class ExchangeListDetail : BaseEntity<int>
    {
        public int ExchangeListId { get; set; }

        public EnumListDetailType DetailType { get; set; }

        public DateTime Time { get; set; }

        public string BarcodeNo { get; set; }

        public string SendDepartmentName { get; set; }

        public string ReceiveDepartmentName { get; set; }

        public string SecSecretLevelText { get; set; }

        public string UrgencyLevelText { get; set; }

        public string CustomData { get; set; }
    }
}
