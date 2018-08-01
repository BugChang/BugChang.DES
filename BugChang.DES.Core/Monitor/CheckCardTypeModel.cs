using System;
using System.Collections.Generic;
using System.Text;

namespace BugChang.DES.Core.Monitor
{
   public class CheckCardTypeModel
    {
        //证卡类型
        public int Type { get; set; } = 0;

        //单位名称
        public string DepartmentName { get; set; }

        //用户名称
        public string UserName { get; set; }

        //代管箱格
        public List<int> Boxs { get; set; }
    }
}
