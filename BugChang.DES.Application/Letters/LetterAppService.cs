using System;
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

namespace BugChang.DES.Application.Letters
{
    public class LetterAppService : ILetterAppService
    {
        private readonly DepartmentManager _departmentManager;
        private readonly SerialNumberManager _serialNumberManager;
        private readonly BarcodeManager _barcodeManager;
        private readonly ILetterRepository _letterRepository;
        private readonly UnitOfWork _unitOfWork;

        public LetterAppService(DepartmentManager departmentManager, SerialNumberManager serialNumberManager, BarcodeManager barcodeManager, ILetterRepository letterRepository, UnitOfWork unitOfWork)
        {
            _departmentManager = departmentManager;
            _serialNumberManager = serialNumberManager;
            _barcodeManager = barcodeManager;
            _letterRepository = letterRepository;
            _unitOfWork = unitOfWork;
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
            var letter = Mapper.Map<Letter>(receiveLetter);
            await _letterRepository.AddAsync(letter);
            result.Success = true;
            await _unitOfWork.CommitAsync();
            result.Data = letter.Id;
            return result;
        }
    }
}
