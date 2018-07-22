using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Core.Commons;
using Microsoft.EntityFrameworkCore;

namespace BugChang.DES.Core.Exchanges.Rules
{
    public class RuleManager
    {
        private readonly IRuleRepository _ruleRepository;

        public RuleManager(IRuleRepository barcodeRuleRepository)
        {
            _ruleRepository = barcodeRuleRepository;
        }

        public async Task<ResultEntity> AddOrUpdateAsync(Rule barcodeRule)
        {
            var result = new ResultEntity();
            var exist = await _ruleRepository.GetQueryable().Where(a => a.Name == barcodeRule.Name && a.Id != barcodeRule.Id).CountAsync() > 0;
            if (exist)
            {
                result.Message = "规则名称已存在";
            }
            else
            {
                if (barcodeRule.Id > 0)
                {
                    _ruleRepository.Update(barcodeRule);
                }
                else
                {
                    await _ruleRepository.AddAsync(barcodeRule);
                }

                result.Success = true;
            }

            return result;
        }

        public async Task<ResultEntity> DeleteAsync(int id)
        {
            var result = new ResultEntity();
            var barcodeRule = await _ruleRepository.GetByIdAsync(id);
            barcodeRule.IsDeleted = true;
            result.Success = true;
            result.Data = barcodeRule;
            return result;
        }
    }
}
