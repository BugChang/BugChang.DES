using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BugChang.DES.Core.SerialNumbers;

namespace BugChang.DES.Application.SerialNumber
{
    public interface ISerialNumberAppService
    {
        Task<int> GetSerialNumber(int departmentId, EnumSerialNumberType type);
    }
}
