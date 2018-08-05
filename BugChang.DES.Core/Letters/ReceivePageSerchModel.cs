using BugChang.DES.Core.Commons;

namespace BugChang.DES.Core.Letters
{
    public class LetterPageSerchModel : PageSearchDetailModel
    {
        public int ReceiveDepartmentId { get; set; }

        public string LetterNo { get; set; }

        public int SendDepartmentId { get; set; }

        public string ShiJiNo { get; set; }
    }
}
