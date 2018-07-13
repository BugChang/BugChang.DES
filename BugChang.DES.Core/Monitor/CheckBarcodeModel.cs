using System.Collections.Generic;

namespace BugChang.DES.Core.Monitor
{
    public class CheckBarcodeModel
    {
        public EnumCheckBarcodeType Type { get; set; } = EnumCheckBarcodeType.无效;

        public IList<CheckedBarcodeRecord> Record { get; set; } = new List<CheckedBarcodeRecord>();
    }

    public class CheckedBarcodeRecord
    {
        public int No { get; set; }

        public int FileCount { get; set; }

        public string Message { get; set; }
    }
}
