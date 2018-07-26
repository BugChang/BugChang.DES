using BugChang.DES.Core.Commons;

namespace BugChang.DES.Core.HardWares
{
    public class HardWare : BaseEntity<int>
    {
        public string DeviceCode { get; set; }

        public EnumHardWareType HardWareType { get; set; }

        public string Value { get; set; }

        public string BaudRate { get; set; }
    }
}
