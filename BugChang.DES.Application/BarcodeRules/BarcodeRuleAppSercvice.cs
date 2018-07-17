using System;
using System.Threading.Tasks;
using AutoMapper;
using BugChang.DES.Application.BarcodeRules.Dtos;
using BugChang.DES.Core.Commons;
using BugChang.DES.Core.Exchanges.Barcodes;
using BugChang.DES.Core.Logs;
using BugChang.DES.EntityFrameWorkCore;
using Newtonsoft.Json;

namespace BugChang.DES.Application.BarcodeRules
{
    public class BarcodeRuleAppSercvice : IBarcodeRuleAppService
    {

        private readonly BarcodeRuleManager _barcodeRuleManager;
        private readonly IBarcodeRuleRepository _barcodeRuleRepository;
        private readonly UnitOfWork _unitOfWork;
        private readonly LogManager _logManager;

        public BarcodeRuleAppSercvice(BarcodeRuleManager barcodeRuleManager, UnitOfWork unitOfWork, LogManager logManager, IBarcodeRuleRepository barcodeRuleRepository)
        {
            _barcodeRuleManager = barcodeRuleManager;
            _unitOfWork = unitOfWork;
            _logManager = logManager;
            _barcodeRuleRepository = barcodeRuleRepository;
        }

        public async Task<ResultEntity> AddOrUpdateAsync(BarcodeRuleEditDto editDto)
        {
            var barcodeRule = Mapper.Map<BarcodeRule>(editDto);
            var result = await _barcodeRuleManager.AddOrUpdateAsync(barcodeRule);
            if (result.Success)
            {
                await _unitOfWork.CommitAsync();
                await _logManager.LogInfomationAsync(EnumLogType.Audit, LogTitleConstString.BarcodeRuleAdd,
                    $"【{editDto.Name}】", JsonConvert.SerializeObject(barcodeRule), editDto.CreateBy);
            }
            return result;
        }

        public Task<ResultEntity> DeleteByIdAsync(int id, int userId)
        {
            throw new NotImplementedException();
        }

        public async Task<BarcodeRuleEditDto> GetForEditByIdAsync(int id)
        {
            var barcodeRule = await _barcodeRuleRepository.GetByIdAsync(id);
            return Mapper.Map<BarcodeRuleEditDto>(barcodeRule);
        }

        public async Task<PageResultModel<BarcodeRuleListDto>> GetPagingAysnc(PageSearchModel pageSearchDto)
        {
            return Mapper.Map<PageResultModel<BarcodeRuleListDto>>(await _barcodeRuleRepository.GetPagingAysnc(pageSearchDto));
        }
    }
}
