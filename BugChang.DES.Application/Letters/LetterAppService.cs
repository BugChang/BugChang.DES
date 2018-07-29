using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BugChang.DES.Application.Letters.Dtos;
using BugChang.DES.Core.Commons;
using BugChang.DES.Core.Departments;
using BugChang.DES.Core.Exchanges.Barcodes;
using BugChang.DES.Core.Letters;
using BugChang.DES.Core.SecretLevels;
using BugChang.DES.Core.SerialNumbers;
using BugChang.DES.Core.Tools;
using BugChang.DES.Core.UrgentLevels;
using BugChang.DES.EntityFrameWorkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BugChang.DES.Application.Letters
{
    public class LetterAppService : ILetterAppService
    {
        private readonly DepartmentManager _departmentManager;
        private readonly SerialNumberManager _serialNumberManager;
        private readonly BarcodeManager _barcodeManager;
        private readonly ILetterRepository _letterRepository;
        private readonly IBarcodeRepository _barcodeRepository;
        private readonly IBarcodeLogRepository _barcodeLogRepository;
        private readonly UnitOfWork _unitOfWork;
        private readonly IOptions<CommonSettings> _commonSettings;

        public LetterAppService(DepartmentManager departmentManager, SerialNumberManager serialNumberManager, BarcodeManager barcodeManager, ILetterRepository letterRepository, UnitOfWork unitOfWork, IOptions<CommonSettings> commonSettings, IBarcodeLogRepository barcodeLogRepository, IBarcodeRepository barcodeRepository)
        {
            _departmentManager = departmentManager;
            _serialNumberManager = serialNumberManager;
            _barcodeManager = barcodeManager;
            _letterRepository = letterRepository;
            _unitOfWork = unitOfWork;
            _commonSettings = commonSettings;
            _barcodeLogRepository = barcodeLogRepository;
            _barcodeRepository = barcodeRepository;
        }

        public Task<ReceiveLetterEditDto> GetReceiveLetter(int letterId)
        {
            throw new NotImplementedException();
        }

        public Task<SendLetterEditDto> GetSendLetter(int letterId)
        {
            throw new NotImplementedException();
        }

        public async Task<ResultEntity> AddReceiveLetter(ReceiveLetterEditDto receiveLetter)
        {
            var result = new ResultEntity();
            var sendDepartmentCode = TextHelper.RepairZeroRight(await _departmentManager.GetDepartmentCode(receiveLetter.SendDepartmentId), 11);
            var receiveDepartmentCode = TextHelper.RepairZeroRight(await _departmentManager.GetDepartmentCode(receiveLetter.ReceiveDepartmentId), 11);
            var serialNumber = await _serialNumberManager.GetSerialNumber(receiveLetter.SendDepartmentId, EnumSerialNumberType.内部交换);
            var barcodeNo = _barcodeManager.MakeBarcodeLength33(sendDepartmentCode, receiveDepartmentCode,
                (EnumSecretLevel)receiveLetter.SecretLevel, (EnumUrgentLevel)receiveLetter.UrgencyLevel,
                serialNumber);
            receiveLetter.BarcodeNo = barcodeNo;
            receiveLetter.LetterNo = barcodeNo.Substring(15, 7);
            var letter = Mapper.Map<Letter>(receiveLetter);
            await _letterRepository.AddAsync(letter);

            var barcode = new Barcode
            {
                BarcodeNo = barcodeNo,
                Entity = EnumBarcodeEntity.信件,
                Souce = EnumBarcodeSouce.本系统,
                Status = EnumBarcodeStatus.已就绪,
                SubStatus = EnumBarcodeSubStatus.正常,
                CreateTime = DateTime.Now,
                CreateBy = receiveLetter.CreateBy
            };
            barcode.BarcodeType = barcode.AnalysisBarcodeType(barcodeNo);
            await _barcodeRepository.AddAsync(barcode);

            var baroceLog = new BarcodeLog
            {
                BarcodeNumber = barcodeNo,
                BarcodeStatus = EnumBarcodeStatus.已就绪,
                DepartmentId = letter.SendDepartmentId,
                OperationTime = DateTime.Now,
                OperatorId = receiveLetter.CreateBy,
            };

            await _barcodeLogRepository.AddAsync(baroceLog);

            result.Success = true;
            await _unitOfWork.CommitAsync();
            result.Data = letter.Id;
            return result;
        }

        public async Task<PageResultModel<LetterReceiveListDto>> GetTodayReceiveLetters(PageSearchCommonModel pageSearchModel)
        {
            var letters = await _letterRepository.GetTodayReceiveLetters(pageSearchModel);
            return Mapper.Map<PageResultModel<LetterReceiveListDto>>(letters);
        }

        public async Task<LetterReceiveBarcodeDto> GetReceiveBarcode(int letterId)
        {
            var letter = await _letterRepository.GetQueryable().Include(a => a.ReceiveDepartment).Include(a => a.SendDepartment).SingleOrDefaultAsync(a => a.Id == letterId);
            return Mapper.Map<LetterReceiveBarcodeDto>(letter);
        }

        public async Task<PageResultModel<LetterReceiveListDto>> GetReceiveLetters(ReceivePageSerchModel pageSearchModel)
        {
            PageResultModel<Letter> letters;
            if (_commonSettings.Value.ReceiveDepartmentId == pageSearchModel.ReceiveDepartmentId)
            {
                letters = await _letterRepository.GetManagerReceiveLetters(pageSearchModel);
            }
            else
            {
                letters = await _letterRepository.GetReceiveLetters(pageSearchModel);
            }

            return Mapper.Map<PageResultModel<LetterReceiveListDto>>(letters);
        }
    }
}
