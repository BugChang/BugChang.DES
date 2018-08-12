using System.Collections.Generic;
using BugChang.DES.Application.Bills.Dtos;

namespace BugChang.DES.Web.Mvc.Models.Bill
{
    public class DeatailViewModel
    {
        public BillDto Bill { get; set; }

        public IList<BillDetailDto> BillDetails { get; set; }
    }
}
