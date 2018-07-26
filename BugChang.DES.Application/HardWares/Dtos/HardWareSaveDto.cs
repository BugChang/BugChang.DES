using System;
using System.Collections.Generic;
using System.Text;

namespace BugChang.DES.Application.HardWares.Dtos
{
   public class HardWareSaveDto
    {
        public string DeviceCode { get; set; }

        public int HardWareType { get; set; }

        public string Value { get; set; }

        public string BaudRate { get; set; }
    }
}
