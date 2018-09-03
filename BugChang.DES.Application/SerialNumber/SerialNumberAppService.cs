using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BugChang.DES.Core.SerialNumbers;
using BugChang.DES.EntityFrameWorkCore;

namespace BugChang.DES.Application.SerialNumber
{
    public class SerialNumberAppService : ISerialNumberAppService
    {
        private readonly SerialNumberManager _serialNumberManager;
        private readonly UnitOfWork _unitOfWork;
        public SerialNumberAppService(SerialNumberManager serialNumberManager, UnitOfWork unitOfWork)
        {
            _serialNumberManager = serialNumberManager;
            _unitOfWork = unitOfWork;
        }

        public async Task<int> GetSerialNumber(int departmentId, EnumSerialNumberType type)
        {
            var serialNo = await _serialNumberManager.GetSerialNumber(departmentId, type);
            if (serialNo != 0)
            {
                await _unitOfWork.CommitAsync();
            }

            return serialNo;
        }
    }
}
