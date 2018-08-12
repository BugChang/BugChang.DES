using System;
using BugChang.DES.Core.Exchanges.Bill;

namespace BugChang.DES.Application.Bills.Dtos
{
    public class BillDetailDto
    {
        public int Id { get; set; }
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
