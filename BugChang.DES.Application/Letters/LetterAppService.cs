using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BugChang.DES.Application.Letters.Dtos;
using BugChang.DES.Core.Commons;
using BugChang.DES.Core.Departments;
using BugChang.DES.Core.Exchanges.Barcodes;
using BugChang.DES.Core.Exchanges.Channel;
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
        private readonly IBackLetterRepository _backLetterRepository;
        private readonly UnitOfWork _unitOfWork;
        private readonly IOptions<CommonSettings> _commonSettings;

        public LetterAppService(DepartmentManager departmentManager, SerialNumberManager serialNumberManager, BarcodeManager barcodeManager, ILetterRepository letterRepository, UnitOfWork unitOfWork, IOptions<CommonSettings> commonSettings, IBarcodeLogRepository barcodeLogRepository, IBarcodeRepository barcodeRepository, IBackLetterRepository backLetterRepository)
        {
            _departmentManager = departmentManager;
            _serialNumberManager = serialNumberManager;
            _barcodeManager = barcodeManager;
            _letterRepository = letterRepository;
            _unitOfWork = unitOfWork;
            _commonSettings = commonSettings;
            _barcodeLogRepository = barcodeLogRepository;
            _barcodeRepository = barcodeRepository;
            _backLetterRepository = backLetterRepository;
        }

        public Task<ReceiveLetterEditDto> GetReceiveLetter(int letterId)
        {
            throw new NotImplementedException();
        }

        public Task<LetterSendEditDto> GetSendLetter(int letterId)
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
            if (barcodeNo == "")
            {
                result.Message = "条码生成失败";
                return result;
            }
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

        public async Task<PageResultModel<LetterReceiveListDto>> GetReceiveLetters(LetterPageSerchModel pageSearchModel)
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

        public async Task<ResultEntity> AddSendLetter(LetterSendEditDto sendLetter)
        {
            var result = new ResultEntity();
            var letter = Mapper.Map<Letter>(sendLetter);
            var receiveDepartment = await _departmentManager.GetAsync(sendLetter.ReceiveDepartmentId);
            var sendDepartmentCode = TextHelper.RepairZeroRight(await _departmentManager.GetDepartmentCode(sendLetter.SendDepartmentId), 11);
            var receiveDepartmentCode = TextHelper.RepairZeroRight(await _departmentManager.GetDepartmentCode(sendLetter.ReceiveDepartmentId), 11);
            letter.LetterType = letter.GetSendLetterType(sendDepartmentCode, receiveDepartmentCode);
            int serialNumber;
            var barcodeNo = "";
            switch (letter.LetterType)
            {
                case EnumLetterType.发信:
                    switch (receiveDepartment.ReceiveChannel)
                    {
                        case EnumChannel.同城交换:
                            serialNumber = await _serialNumberManager.GetSerialNumber(letter.SendDepartmentId, EnumSerialNumberType.同城交换);
                            barcodeNo = _barcodeManager.MakeBarcodeLength26(sendDepartmentCode, receiveDepartmentCode,
                                letter.SecretLevel, letter.UrgencyLevel,
                                serialNumber);
                            break;
                        case EnumChannel.机要通信:
                            serialNumber = await _serialNumberManager.GetSerialNumber(letter.SendDepartmentId, EnumSerialNumberType.机要通信);
                            barcodeNo = _barcodeManager.MakeBarcodeLength33(sendDepartmentCode, receiveDepartmentCode,
                                letter.SecretLevel, letter.UrgencyLevel,
                                serialNumber);
                            break;
                    }
                    break;
                case EnumLetterType.内交换:
                    serialNumber = await _serialNumberManager.GetSerialNumber(letter.SendDepartmentId, EnumSerialNumberType.内部交换);
                    barcodeNo = _barcodeManager.MakeBarcodeLength33(sendDepartmentCode, receiveDepartmentCode,
                        letter.SecretLevel, letter.UrgencyLevel,
                        serialNumber);
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"发信类型有误");
            }

            if (barcodeNo == "")
            {
                result.Message = "条码生成失败";
                return result;
            }
            letter.BarcodeNo = barcodeNo;
            letter.LetterNo = barcodeNo.Length == 33 ? barcodeNo.Substring(15, 7) : barcodeNo.Substring(8, 8);
            await _letterRepository.AddAsync(letter);

            var barcode = new Barcode
            {
                BarcodeNo = barcodeNo,
                Entity = EnumBarcodeEntity.信件,
                Souce = EnumBarcodeSouce.本系统,
                Status = EnumBarcodeStatus.已就绪,
                SubStatus = EnumBarcodeSubStatus.正常,
                CreateTime = DateTime.Now,
                CreateBy = letter.CreateBy
            };
            barcode.BarcodeType = barcode.AnalysisBarcodeType(barcodeNo);
            await _barcodeRepository.AddAsync(barcode);

            var baroceLog = new BarcodeLog
            {
                BarcodeNumber = barcodeNo,
                BarcodeStatus = EnumBarcodeStatus.已就绪,
                DepartmentId = letter.SendDepartmentId,
                OperationTime = DateTime.Now,
                OperatorId = letter.CreateBy,
            };

            await _barcodeLogRepository.AddAsync(baroceLog);
            result.Success = true;
            await _unitOfWork.CommitAsync();
            result.Data = letter.Id;
            return result;
        }

        public async Task<PageResultModel<LetterSendListDto>> GetTodaySendLetters(PageSearchCommonModel pageSearchModel)
        {
            var letters = await _letterRepository.GetTodaySendLetters(pageSearchModel);
            return Mapper.Map<PageResultModel<LetterSendListDto>>(letters);
        }

        public async Task<PageResultModel<LetterSendListDto>> GetSendLetters(LetterPageSerchModel pageSearchModel)
        {
            var letters = await _letterRepository.GetSendLetters(pageSearchModel);
            return Mapper.Map<PageResultModel<LetterSendListDto>>(letters);
        }

        public async Task<PageResultModel<LetterBackListDto>> GetBackLetters(PageSearchCommonModel pageSearchModel)
        {
            var letters = await _backLetterRepository.GetBackLetters(pageSearchModel);
            return Mapper.Map<PageResultModel<LetterBackListDto>>(letters);
        }

        public async Task<PageResultModel<LetterBackListDto>> GetBackLettersForSearch(PageSearchCommonModel pageSearchModel)
        {
            PageResultModel<Letter> letters;
            if (_commonSettings.Value.ReceiveDepartmentId == pageSearchModel.DepartmentId)
            {
                letters = await _letterRepository.GetBackLettersForManagerSearch(pageSearchModel);
            }
            else
            {
                letters = await _letterRepository.GetBackLettersForSearch(pageSearchModel);
            }

            return Mapper.Map<PageResultModel<LetterBackListDto>>(letters);
        }

        public async Task<ResultEntity> BackLetter(int letterId, int departmentId, int operatorId)
        {
            var result = new ResultEntity();
            var letter = await _letterRepository.GetByIdAsync(letterId);
            var existBack = await _backLetterRepository.GetQueryable().Where(a => a.LetterId == letterId).CountAsync() > 0;
            if (existBack)
            {
                result.Message = "该文件已经存在退回记录，请勿重复操作！";
            }
            else
            {
                var barcode = await _barcodeRepository.GetByNoAsync(letter.BarcodeNo);
                if (barcode == null || barcode.Status!= EnumBarcodeStatus.已签收)
                {
                    result.Message = "流转状态不正确，无法退回！";
                }
                else
                {
                    barcode.Status = EnumBarcodeStatus.申请退回;
                    var backLetter = new BackLetter
                    {
                        LetterId = letterId,
                        OperationDepartmentId = departmentId,
                        OperatorId = operatorId,
                        OperationTime = DateTime.Now
                    };
                    await _backLetterRepository.AddAsync(backLetter);
                    await _unitOfWork.CommitAsync();
                    result.Success = true;
                }
            }

            return result;
        }
    }
}
