using System;
using System.Threading.Tasks;
using BugChang.DES.Application.BarcodeRules.Dtos;
using BugChang.DES.Core.Commons;

namespace BugChang.DES.Application.BarcodeRules
{
    public class BarcodeRuleAppSercvice : IBarcodeRuleAppService
    {
        public Task<ResultEntity> AddOrUpdateAsync(BarcodeRuleEditDto editDto)
        {
            throw new NotImplementedException();
        }

        public Task<ResultEntity> DeleteByIdAsync(int id, int userId)
        {
            throw new NotImplementedException();
        }

        public Task<BarcodeRuleEditDto> GetForEditByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<PageResultModel<BarcodeRuleListDto>> GetPagingAysnc(PageSearchModel pageSearchDto)
        {
            throw new NotImplementedException();
        }
    }
}
