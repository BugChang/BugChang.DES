﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BugChang.DES.Core.SerialNumbers
{
    public class SerialNumberManager
    {
        private readonly ISerialNumberRepository _numberRepository;

        public SerialNumberManager(ISerialNumberRepository numberRepository)
        {
            _numberRepository = numberRepository;
        }

        public async Task<int> GetSerialNumber(int departmentId, EnumSerialNumberType type)
        {
            var serialNumber = await _numberRepository.GetQueryable().Where(a => a.DepartmentId == departmentId && (int)a.Type == (int)type)
                .FirstOrDefaultAsync();
            if (serialNumber == null)
            {
                serialNumber = new SerialNumber
                {
                    DepartmentId = departmentId,
                    Type = type,
                    Year = DateTime.Now.Year,
                    Number = 1
                };
                await _numberRepository.AddAsync(serialNumber);
            }

            var no = serialNumber.Number;
            serialNumber.Number = serialNumber.Number + 1;
            return no;
        }
    }
}
