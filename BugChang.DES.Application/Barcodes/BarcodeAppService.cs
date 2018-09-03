using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BugChang.DES.Application.Barcodes.Dtos;
using BugChang.DES.Core.Exchanges.Barcodes;
using BugChang.DES.Core.SecretLevels;
using BugChang.DES.Core.UrgentLevels;

namespace BugChang.DES.Application.Barcodes
{
    public class BarcodeAppService : IBarcodeAppService
    {
        private readonly BarcodeManager _barcodeManager;

        public BarcodeAppService(BarcodeManager barcodeManager)
        {
            _barcodeManager = barcodeManager;
        }

        public IList<BarcodeTypeListDto> GetBarcodeTypes()
        {
            var barcodeTypeList = new List<BarcodeTypeListDto>();
            foreach (var item in Enum.GetValues(typeof(EnumBarcodeType)))
            {
                var barcodeType = new BarcodeTypeListDto
                {
                    Id = (int)item,
                    Name = item.ToString()
                };
                barcodeTypeList.Add(barcodeType);
            }

            return barcodeTypeList;
        }

        public string MakeBarcodeLength33(string sendDepartmentCode, string receiveDepartmentCode, EnumSecretLevel secretLevel,
             EnumUrgentLevel urgentLevel, int serialNo)
        {
            return _barcodeManager.MakeBarcodeLength33(sendDepartmentCode, receiveDepartmentCode, secretLevel,
                urgentLevel, serialNo);
        }

        public string MakeBarcodeLength26(string sendDepartmentCode, string receiveDepartmentCode,
            EnumSecretLevel secretLevel, EnumUrgentLevel urgentLevel, int serialNo)
        {
            return _barcodeManager.MakeBarcodeLength26(sendDepartmentCode, receiveDepartmentCode, secretLevel,
                urgentLevel, serialNo);
        }
    }
}
