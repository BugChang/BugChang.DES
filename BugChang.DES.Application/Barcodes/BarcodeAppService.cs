using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BugChang.DES.Application.Barcodes.Dtos;
using BugChang.DES.Core.Exchanges.Barcodes;

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

        public async Task<string> MakeBarcodeNo(int recDepartmentId, int sendDepartmentId)
        {
            return await _barcodeManager.MakeInsideBarcode(recDepartmentId, sendDepartmentId);
        }
    }
}
