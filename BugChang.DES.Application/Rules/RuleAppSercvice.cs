using System.Threading.Tasks;
using AutoMapper;
using BugChang.DES.Application.Rules.Dtos;
using BugChang.DES.Core.Commons;
using BugChang.DES.Core.Exchanges.Barcodes;
using BugChang.DES.Core.Exchanges.Rules;
using BugChang.DES.Core.Logs;
using BugChang.DES.EntityFrameWorkCore;
using Newtonsoft.Json;

namespace BugChang.DES.Application.Rules
{
    public class RuleAppSercvice : IRuleAppService
    {

        private readonly RuleManager _barcodeRuleManager;
        private readonly IRuleRepository _ruleRepository;
        private readonly UnitOfWork _unitOfWork;
        private readonly LogManager _logManager;

        public RuleAppSercvice(RuleManager barcodeRuleManager, UnitOfWork unitOfWork, LogManager logManager, IRuleRepository ruleRepository)
        {
            _barcodeRuleManager = barcodeRuleManager;
            _unitOfWork = unitOfWork;
            _logManager = logManager;
            _ruleRepository = ruleRepository;
        }

        public async Task<ResultEntity> AddOrUpdateAsync(RuleEditDto editDto)
        {
            var barcodeRule = Mapper.Map<Rule>(editDto);
            var result = await _barcodeRuleManager.AddOrUpdateAsync(barcodeRule);
            if (result.Success)
            {
                await _unitOfWork.CommitAsync();
                if (editDto.Id > 0)
                {
                    await _logManager.LogInfomationAsync(EnumLogType.Audit, LogTitleConstString.BarcodeRuleEdit,
                        $"【{editDto.Name}】", JsonConvert.SerializeObject(barcodeRule), editDto.CreateBy);
                }
                else
                {
                    await _logManager.LogInfomationAsync(EnumLogType.Audit, LogTitleConstString.BarcodeRuleAdd,
                        $"【{editDto.Name}】", JsonConvert.SerializeObject(barcodeRule), editDto.CreateBy);
                }

            }
            return result;
        }

        public async Task<ResultEntity> DeleteByIdAsync(int id, int userId)
        {
            var result = await _barcodeRuleManager.DeleteAsync(id);
            if (result.Success)
            {
                await _unitOfWork.CommitAsync();
                await _logManager.LogInfomationAsync(EnumLogType.Audit, LogTitleConstString.BarcodeRuleDelete,
                    $"【{result.Data.Name}】已删除", JsonConvert.SerializeObject(result.Data), userId);
            }

            return result;
        }

        public async Task<RuleEditDto> GetForEditByIdAsync(int id)
        {
            var barcodeRule = await _ruleRepository.GetByIdAsync(id);
            return Mapper.Map<RuleEditDto>(barcodeRule);
        }

        public async Task<PageResultModel<RuleListDto>> GetPagingAysnc(PageSearchModel pageSearchDto)
        {
            return Mapper.Map<PageResultModel<RuleListDto>>(await _ruleRepository.GetPagingAysnc(pageSearchDto));
        }
    }
}
