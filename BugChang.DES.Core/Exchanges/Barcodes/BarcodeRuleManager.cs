using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Core.Commons;
using Microsoft.EntityFrameworkCore;

namespace BugChang.DES.Core.Exchanges.Barcodes
{
    public class BarcodeRuleManager
    {
        private readonly IBarcodeRuleRepository _barcodeRuleRepository;

        public BarcodeRuleManager(IBarcodeRuleRepository barcodeRuleRepository)
        {
            _barcodeRuleRepository = barcodeRuleRepository;
        }

        public async Task<ResultEntity> AddOrUpdateAsync(BarcodeRule barcodeRule)
        {
            var result = new ResultEntity();
            var exist = await _barcodeRuleRepository.GetQueryable().Where(a => a.Name == barcodeRule.Name && a.Id != barcodeRule.Id).CountAsync() > 0;
            if (exist)
            {
                result.Message = "规则名称已存在";
            }
            else
            {
                if (barcodeRule.Id > 0)
                {
                   _barcodeRuleRepository.Update(barcodeRule);
                }
                else
                {
                    await _barcodeRuleRepository.AddAsync(barcodeRule);
                }

                result.Success = true;
            }

            return result;
        }
    }
}
