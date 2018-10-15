using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BugChang.DES.Application.Letters.Dtos;
using BugChang.DES.Core.Commons;
using BugChang.DES.Core.Departments;
using BugChang.DES.Core.Exchanges.Barcodes;
using BugChang.DES.Core.Exchanges.Boxs;
using BugChang.DES.Core.Exchanges.Channel;
using BugChang.DES.Core.Exchanges.Places;
using BugChang.DES.Core.Letters;
using BugChang.DES.Core.SecretLevels;
using BugChang.DES.Core.SerialNumbers;
using BugChang.DES.Core.Sortings;
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
        private readonly ICancelLetterRepository _cancelLetterRepository;
        private readonly ISortingRepository _sortingRepository;
        private readonly ISortingListRepository _sortingListRepository;
        private readonly ISortingListDetailRepository _sortingListDetailRepository;
        private readonly BoxManager _boxManager;
        private readonly IPlaceRepository _placeRepository;
        private readonly IPlaceWardenRepository _placeWardenRepository;
        private readonly UnitOfWork _unitOfWork;
        private readonly IOptions<CommonSettings> _commonSettings;

        public LetterAppService(DepartmentManager departmentManager, SerialNumberManager serialNumberManager, BarcodeManager barcodeManager, ILetterRepository letterRepository, UnitOfWork unitOfWork, IOptions<CommonSettings> commonSettings, IBarcodeLogRepository barcodeLogRepository, IBarcodeRepository barcodeRepository, IBackLetterRepository backLetterRepository, ICancelLetterRepository cancelLetterRepository, BoxManager boxManager, ISortingListDetailRepository sortingListDetailRepository, ISortingListRepository sortingListRepository, ISortingRepository sortingRepository, IPlaceRepository placeRepository, IPlaceWardenRepository placeWardenRepository)
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
            _cancelLetterRepository = cancelLetterRepository;
            _boxManager = boxManager;
            _sortingListDetailRepository = sortingListDetailRepository;
            _sortingListRepository = sortingListRepository;
            _sortingRepository = sortingRepository;
            _placeRepository = placeRepository;
            _placeWardenRepository = placeWardenRepository;
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

        public async Task<LetterSendBarcodeDto> GetSendBarcode(int letterId)
        {
            var letter = await _letterRepository.GetQueryable().Include(a => a.ReceiveDepartment).Include(a => a.SendDepartment).SingleOrDefaultAsync(a => a.Id == letterId);
            return Mapper.Map<LetterSendBarcodeDto>(letter);
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
            if (string.IsNullOrWhiteSpace(letter.BarcodeNo))
            {
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
                letter.LetterNo = letter.GetLetterNo(barcodeNo);
            }
            await _letterRepository.AddAsync(letter);

            var barcode = new Barcode
            {
                BarcodeNo = letter.BarcodeNo,
                Entity = EnumBarcodeEntity.信件,
                Souce = EnumBarcodeSouce.本系统,
                Status = EnumBarcodeStatus.已就绪,
                SubStatus = EnumBarcodeSubStatus.正常,
                CreateTime = DateTime.Now,
                CreateBy = letter.CreateBy
            };
            barcode.BarcodeType = barcode.AnalysisBarcodeType(letter.BarcodeNo);
            await _barcodeRepository.AddAsync(barcode);

            var baroceLog = new BarcodeLog
            {
                BarcodeNumber = letter.BarcodeNo,
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
                if (barcode == null || barcode.Status != EnumBarcodeStatus.已签收)
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
                    var barcodeLog = new BarcodeLog
                    {
                        BarcodeNumber = barcode.BarcodeNo,
                        BarcodeStatus = EnumBarcodeStatus.申请退回,
                        CurrentObjectId = barcode.CurrentObjectId,
                        CurrentPlaceId = barcode.CurrentPlaceId,
                        DepartmentId = letter.ReceiveDepartmentId,
                        OperationTime = DateTime.Now,
                        OperatorId = operatorId,
                        Remark = "文件已申请退回"
                    };
                    await _barcodeLogRepository.AddAsync(barcodeLog);
                    await _backLetterRepository.AddAsync(backLetter);
                    await _unitOfWork.CommitAsync();
                    result.Success = true;
                }
            }

            return result;
        }

        public async Task<PageResultModel<LetterCancelListDto>> GetCancelLetters(PageSearchCommonModel pageSearchModel)
        {
            var letters = await _cancelLetterRepository.GetCancelLetters(pageSearchModel);
            return Mapper.Map<PageResultModel<LetterCancelListDto>>(letters);
        }

        public async Task<PageResultModel<LetterCancelListDto>> GetCancelLettersForSearch(PageSearchCommonModel pageSearchModel)
        {
            var letters = await _letterRepository.GetCancelLettersForSearch(pageSearchModel);
            return Mapper.Map<PageResultModel<LetterCancelListDto>>(letters);
        }

        public async Task<ResultEntity> CancelLetter(int letterId, int departmentId, int operatorId, int applicantId)
        {
            var result = new ResultEntity();
            var letter = await _letterRepository.GetByIdAsync(letterId);
            var barcode = await _barcodeRepository.GetByNoAsync(letter.BarcodeNo);
            if (barcode == null || barcode.Status != EnumBarcodeStatus.已投递)
            {
                result.Message = "流转状态不正确，无法勘误！";
            }
            else
            {
                barcode.Status = EnumBarcodeStatus.已勘误;
                var cancelLetter = new CancelLetter
                {
                    LetterId = letterId,
                    OperationDepartmentId = departmentId,
                    OperatorId = operatorId,
                    ApplicantId = applicantId,
                    OperationTime = DateTime.Now
                };
                var barcodeLog = new BarcodeLog
                {
                    BarcodeNumber = barcode.BarcodeNo,
                    BarcodeStatus = EnumBarcodeStatus.已勘误,
                    CurrentObjectId = barcode.CurrentObjectId,
                    CurrentPlaceId = barcode.CurrentPlaceId,
                    DepartmentId = letter.ReceiveDepartmentId,
                    OperationTime = DateTime.Now,
                    OperatorId = applicantId,
                    Remark = "文件已勘误"
                };

                result = await _boxManager.Cancel(barcode.CurrentObjectId);
                if (result.Success)
                {
                    await _barcodeLogRepository.AddAsync(barcodeLog);
                    await _cancelLetterRepository.AddAsync(cancelLetter);
                    await _unitOfWork.CommitAsync();
                }
            }
            return result;
        }

        public async Task<PageResultModel<LetterSortingDto>> GetNoSortingLetters(EnumChannel channel)
        {
            var letters = await _letterRepository.GetNoSortingLetters(channel);
            return Mapper.Map<PageResultModel<LetterSortingDto>>(letters);
        }

        public async Task<ResultEntity> CreateSortingList(EnumChannel channel, List<int> letterIds, int userId)
        {
            var result = new ResultEntity();
            var letters = await _letterRepository.GetQueryable().Where(a => letterIds.Contains(a.Id)).ToListAsync();
            var sortings = _sortingRepository.GetQueryable().Where(a => letters.Any(b => b.BarcodeNo == a.BarcodeNo));
            foreach (var sorting in sortings)
            {
                sorting.Sorted = true;
            }

            var serialNumber = await _serialNumberManager.GetSerialNumber(0, EnumSerialNumberType.分拣);
            var sortingList = new SortingList
            {
                Channel = channel,
                ListNo = DateTime.Now.Year + serialNumber.ToString("00000000"),
                CreateBy = userId,
                CreateTime = DateTime.Now
            };
            await _sortingListRepository.AddAsync(sortingList);
            foreach (var letterId in letterIds)
            {
                var listDetail = new SortingListDetail
                {
                    LetterId = letterId,
                    SortingList = sortingList
                };
                await _sortingListDetailRepository.AddAsync(listDetail);
            }

            if (await _unitOfWork.CommitAsync() > 0)
            {
                result.Success = true;
                result.Data = sortingList.Id;
            }
            return result;
        }

        public async Task<ResultEntity> Change2Jytx(int letterId)
        {

            var letter = await _letterRepository.GetByIdAsync(letterId);
            var sorting = await _sortingRepository.GetQueryable().FirstOrDefaultAsync(a => a.BarcodeNo == letter.BarcodeNo);
            sorting.Channel = EnumChannel.机要通信;
            await _unitOfWork.CommitAsync();
            return new ResultEntity { Success = true };
        }

        public async Task<ResultEntity> GetWriteCpuCardData(int listId)
        {

            var list = await _sortingListRepository.GetQueryable().FirstOrDefaultAsync(a => a.Id == listId);
            var listDetails = await _sortingListDetailRepository.GetQueryable().Where(a => a.SortingListId == list.Id)
                .ToListAsync();
            var letters = await _letterRepository.GetQueryable().Where(a => listDetails.Any(b => b.LetterId == a.Id))
                .ToListAsync();
            //1.存储标识头
            var head = "";
            //1)标识头
            head += "JH$";
            //2)写卡单位
            head += _commonSettings.Value.UseDepartmentCode + "$";
            //3)读卡单位
            head += _commonSettings.Value.ReadCardDepartmentCode + "$";
            //4)操作时间
            head += DateTime.Now.ToString("yyyyMMddHHmm") + "$";
            //5)多张卡时需要，暂时忽略
            head += "$" + "1$" + "1$";
            //6)数据存储格式:0.原格式1.压缩格式
            head += "0$";
            //7)数据长度


            //2.存储数据格式 
            var body = "YF$";
            body += Convert.ToInt32(list.ListNo.Substring(4)) + "$";
            body += DateTime.Now.ToString("yyMMdd") + "^";

            foreach (var detail in letters)
            {
                body += detail.BarcodeNo;
                if (detail != letters[letters.Count - 1])
                {
                    body += "$";
                }
            }

            var bodyByte = System.Text.Encoding.Default.GetBytes(body);
            head += bodyByte.LongLength;

            var writeCardData = head + "|" + body + "||";
            return new ResultEntity
            {
                Success = true,
                Data = writeCardData
            };
        }

        public async Task<IList<LetterSortingDto>> GetSortListDetails(int listId)
        {
            var list = await _sortingListRepository.GetQueryable().FirstOrDefaultAsync(a => a.Id == listId);
            var listDetails = await _sortingListDetailRepository.GetQueryable().Where(a => a.SortingListId == list.Id).Select(a => a.Letter).Include(a => a.ReceiveDepartment).Include(a => a.SendDepartment)
                .ToListAsync();
            return Mapper.Map<IList<LetterSortingDto>>(listDetails);
        }

        public async Task<int> GetLetterIdByBarcodeNo(string barcodeNo)
        {
            var letter = await _letterRepository.GetQueryable().FirstOrDefaultAsync(a => a.BarcodeNo == barcodeNo);
            if (letter == null)
            {
                return 0;
            }

            return letter.Id;
        }

        public async Task<SortingListDto> GetSortingList(int listId)
        {
            var sortingList = await _sortingListRepository.GetQueryable().Include(a => a.CreateUser)
                .FirstOrDefaultAsync(a => a.Id == listId);
            return Mapper.Map<SortingListDto>(sortingList);
        }

        public async Task<PageResultModel<SortingListDto>> GetSortingLists(PageSearchCommonModel pageSearch)
        {
            var sortings = _sortingListRepository.GetQueryable().Include(a => a.CreateUser).Include(a => a.UpdateUser).Where(a => true);
            if (!string.IsNullOrWhiteSpace(pageSearch.Keywords))
            {
                sortings = sortings.Where(a => a.ListNo.Contains(pageSearch.Keywords));
            }
            var pageResult = new PageResultModel<SortingList>
            {
                Rows = await sortings.OrderByDescending(a => a.Id).Skip(pageSearch.Skip).Take(pageSearch.Take).ToListAsync(),
                Total = await sortings.CountAsync()
            };
            return Mapper.Map<PageResultModel<SortingListDto>>(pageResult);
        }

        public async Task<ResultEntity> GetCheckInfo(int userId, int placeId, DateTime beginTime, DateTime endTime)
        {
            var result = new ResultEntity();
            var listInfo = new List<string>();
            var managePlace = await _placeWardenRepository.GetQueryable().FirstOrDefaultAsync(a => a.UserId == userId);
            var place = await _placeRepository.GetQueryable().Include(a => a.Parent).FirstOrDefaultAsync(a => a.Id == placeId);
            if (managePlace == null)
            {
                result.Message = "非场所管理员，无法使用此功能";
                return result;
            }

            if (managePlace.PlaceId == placeId)
            {
                if (place.ParentId == null)
                {
                    result.Message = "使用场所错误，无法核销";
                }
                else
                {
                    var receiveLogs = await _barcodeLogRepository.GetQueryable().Where(a =>
                            a.BarcodeStatus == EnumBarcodeStatus.已签收 && a.CurrentPlaceId == place.ParentId &&
                            a.OperationTime.Date >= beginTime.Date && a.OperationTime.Date <= endTime.Date)
                        .ToListAsync();
                    listInfo.Add($"场所【{place.Parent.Name}】取件【{receiveLogs.Count}】");
                    var sendLogs = await _barcodeLogRepository.GetQueryable().Where(a =>
                        a.BarcodeStatus == EnumBarcodeStatus.已投递 && a.CurrentPlaceId == placeId &&
                        a.OperationTime.Date >= beginTime.Date && a.OperationTime.Date <= endTime.Date &&
                        receiveLogs.Exists(b => b.BarcodeNumber == a.BarcodeNumber)).ToListAsync();
                    listInfo.Add($"场所【{place.Name}】投件【{sendLogs.Count}】");
                    listInfo.Add(receiveLogs.Count == sendLogs.Count
                        ? "成功！所有文件已成功投递"
                        : $"警告！{receiveLogs.Count - sendLogs.Count}份文件尚未投递，请查看下方明细！");
                }
            }
            else
            {
                var receiveLogs = await _barcodeLogRepository.GetQueryable().Where(a =>
                        a.BarcodeStatus == EnumBarcodeStatus.已签收 && a.CurrentPlaceId == placeId &&
                        a.OperationTime.Date >= beginTime.Date && a.OperationTime.Date <= endTime.Date)
                    .ToListAsync();
                listInfo.Add($"场所【{place.Name}】取件【{receiveLogs.Count}】");
                var sendLogs = await _barcodeLogRepository.GetQueryable().Where(a =>
                    a.BarcodeStatus == EnumBarcodeStatus.已投递 && a.CurrentPlaceId == place.ParentId &&
                    a.OperationTime.Date >= beginTime.Date && a.OperationTime.Date <= endTime.Date &&
                    receiveLogs.Exists(b => b.BarcodeNumber == a.BarcodeNumber)).ToListAsync();
                listInfo.Add($"场所【{place.Parent.Name}】投件【{sendLogs.Count}】");
                listInfo.Add(receiveLogs.Count == sendLogs.Count
                    ? "成功！所有文件已成功投递"
                    : $"警告！{receiveLogs.Count - sendLogs.Count}份文件尚未投递，请查看下方明细！");

            }
            result.Success = true;
            result.Data = listInfo;
            return result;
        }

        public async Task<PageResultModel<LetterCheckListDto>> GetCheckLetters(PageSearchDetailModel pageSearch)
        {
            List<Letter> letters;
            var managePlace = await _placeWardenRepository.GetQueryable().FirstOrDefaultAsync(a => a.UserId == pageSearch.UserId);
            var place = await _placeRepository.GetQueryable().Include(a => a.Parent).FirstOrDefaultAsync(a => a.Id == pageSearch.PlaceId);

            if (managePlace.PlaceId == pageSearch.PlaceId)
            {

                var receiveLogs = await _barcodeLogRepository.GetQueryable().Where(a =>
                        a.BarcodeStatus == EnumBarcodeStatus.已签收 && a.CurrentPlaceId == place.ParentId &&
                        a.OperationTime.Date >= pageSearch.BeginTime.Value.Date && a.OperationTime.Date <= pageSearch.EndTime.Value.Date)
                    .ToListAsync();

                var sendLogs = await _barcodeLogRepository.GetQueryable().Where(a =>
                    a.BarcodeStatus == EnumBarcodeStatus.已投递 && a.CurrentPlaceId == pageSearch.PlaceId &&
                    a.OperationTime.Date >= pageSearch.BeginTime.Value.Date && a.OperationTime.Date <= pageSearch.EndTime.Value.Date &&
                    receiveLogs.Exists(b => b.BarcodeNumber == a.BarcodeNumber)).ToListAsync();
                letters = await _letterRepository.GetQueryable()
                    .Where(a => sendLogs.Exists(b => b.BarcodeNumber == a.BarcodeNo)).ToListAsync();
            }
            else
            {
                var receiveLogs = await _barcodeLogRepository.GetQueryable().Where(a =>
                        a.BarcodeStatus == EnumBarcodeStatus.已签收 && a.CurrentPlaceId == pageSearch.PlaceId &&
                        a.OperationTime.Date >= pageSearch.BeginTime.Value.Date && a.OperationTime.Date <= pageSearch.EndTime.Value.Date)
                    .ToListAsync();
                var sendLogs = await _barcodeLogRepository.GetQueryable().Where(a =>
                    a.BarcodeStatus == EnumBarcodeStatus.已投递 && a.CurrentPlaceId == place.ParentId &&
                    a.OperationTime.Date >= pageSearch.BeginTime.Value.Date && a.OperationTime.Date <= pageSearch.EndTime.Value.Date &&
                    receiveLogs.Exists(b => b.BarcodeNumber == a.BarcodeNumber)).ToListAsync();
                letters = await _letterRepository.GetQueryable()
                    .Where(a => sendLogs.Exists(b => b.BarcodeNumber == a.BarcodeNo)).ToListAsync();
            }
            var pageLetters = new PageResultModel<Letter>
            {
                Rows = letters,
                Total = letters.Count
            };
            return Mapper.Map<PageResultModel<LetterCheckListDto>>(pageLetters);
        }

        public async Task<PageResultModel<LetterReceiveListDto>> Out2InsideLetters(PageSearchCommonModel pageSearch)
        {
            var barcodeLogs = await _barcodeLogRepository.GetQueryable().Where(a =>
                 a.OperationTime.Date == DateTime.Now.Date &&
                 (a.DepartmentId == _commonSettings.Value.ReceiveDepartmentId ||
                  a.DepartmentId == _commonSettings.Value.UseDepartmentId) && a.BarcodeStatus == EnumBarcodeStatus.已投递 &&
                 a.CurrentPlaceId == _commonSettings.Value.RootPlaceId).ToListAsync();

            var letters = await _letterRepository.GetQueryable().Include(a => a.ReceiveDepartment).Include(a => a.SendDepartment)
                .Where(a => barcodeLogs.Exists(b => b.BarcodeNumber == a.BarcodeNo)).ToListAsync();

            var pageResult = new PageResultModel<Letter>
            {
                Rows = letters,
                Total = letters.Count
            };
            return Mapper.Map<PageResultModel<LetterReceiveListDto>>(pageResult);
        }

        public async Task<DepartmentStatisticsDto> GetDepartmentStatistics(int departmentId, DateTime beginDate, DateTime endDate)
        {

            var receive = from barcodeLog in _barcodeLogRepository.GetQueryable()
                          join letter in _letterRepository.GetQueryable() on barcodeLog.BarcodeNumber equals letter.BarcodeNo
                          where barcodeLog.BarcodeStatus == EnumBarcodeStatus.已签收 &&
                                barcodeLog.OperationTime.Date >= beginDate.Date && barcodeLog.OperationTime <= endDate.Date &&
                                barcodeLog.DepartmentId == departmentId
                          select letter;

            var receiveOut = from barcodeLog in _barcodeLogRepository.GetQueryable()
                             join letter in _letterRepository.GetQueryable() on barcodeLog.BarcodeNumber equals letter.BarcodeNo
                             where barcodeLog.BarcodeStatus == EnumBarcodeStatus.已签收 &&
                                   barcodeLog.OperationTime.Date >= beginDate.Date && barcodeLog.OperationTime <= endDate.Date &&
                                   barcodeLog.DepartmentId == departmentId
                                   && letter.LetterType == EnumLetterType.收信
                             select letter;

            var send = from barcodeLog in _barcodeLogRepository.GetQueryable()
                       join letter in _letterRepository.GetQueryable() on barcodeLog.BarcodeNumber equals letter.BarcodeNo
                       where barcodeLog.BarcodeStatus == EnumBarcodeStatus.已投递 &&
                             barcodeLog.OperationTime.Date >= beginDate.Date && barcodeLog.OperationTime <= endDate.Date &&
                             barcodeLog.DepartmentId == departmentId
                       select letter;

            var sendOut = from barcodeLog in _barcodeLogRepository.GetQueryable()
                          join letter in _letterRepository.GetQueryable() on barcodeLog.BarcodeNumber equals letter.BarcodeNo
                          where barcodeLog.BarcodeStatus == EnumBarcodeStatus.已投递 &&
                                barcodeLog.OperationTime.Date >= beginDate.Date && barcodeLog.OperationTime <= endDate.Date &&
                                barcodeLog.DepartmentId == departmentId
                                && letter.LetterType == EnumLetterType.发信
                          select letter;

            var sec0 = from barcodeLog in _barcodeLogRepository.GetQueryable()
                       join letter in _letterRepository.GetQueryable() on barcodeLog.BarcodeNumber equals letter.BarcodeNo
                       where barcodeLog.OperationTime.Date >= beginDate.Date && barcodeLog.OperationTime <= endDate.Date &&
                             barcodeLog.DepartmentId == departmentId
                             && letter.SecretLevel == EnumSecretLevel.无
                       select letter;

            var sec1 = from barcodeLog in _barcodeLogRepository.GetQueryable()
                       join letter in _letterRepository.GetQueryable() on barcodeLog.BarcodeNumber equals letter.BarcodeNo
                       where barcodeLog.OperationTime.Date >= beginDate.Date && barcodeLog.OperationTime <= endDate.Date &&
                             barcodeLog.DepartmentId == departmentId
                             && letter.SecretLevel == EnumSecretLevel.秘密
                       select letter;

            var sec2 = from barcodeLog in _barcodeLogRepository.GetQueryable()
                       join letter in _letterRepository.GetQueryable() on barcodeLog.BarcodeNumber equals letter.BarcodeNo
                       where barcodeLog.OperationTime.Date >= beginDate.Date && barcodeLog.OperationTime <= endDate.Date &&
                             barcodeLog.DepartmentId == departmentId
                             && letter.SecretLevel == EnumSecretLevel.机密
                       select letter;

            var sec3 = from barcodeLog in _barcodeLogRepository.GetQueryable()
                       join letter in _letterRepository.GetQueryable() on barcodeLog.BarcodeNumber equals letter.BarcodeNo
                       where barcodeLog.OperationTime.Date >= beginDate.Date && barcodeLog.OperationTime <= endDate.Date &&
                             barcodeLog.DepartmentId == departmentId
                             && letter.SecretLevel == EnumSecretLevel.绝密
                       select letter;

            var urg0 = from barcodeLog in _barcodeLogRepository.GetQueryable()
                       join letter in _letterRepository.GetQueryable() on barcodeLog.BarcodeNumber equals letter.BarcodeNo
                       where barcodeLog.OperationTime.Date >= beginDate.Date && barcodeLog.OperationTime <= endDate.Date &&
                             barcodeLog.DepartmentId == departmentId
                             && letter.UrgencyLevel == EnumUrgentLevel.无
                       select letter;

            var urg1 = from barcodeLog in _barcodeLogRepository.GetQueryable()
                       join letter in _letterRepository.GetQueryable() on barcodeLog.BarcodeNumber equals letter.BarcodeNo
                       where barcodeLog.OperationTime.Date >= beginDate.Date && barcodeLog.OperationTime <= endDate.Date &&
                             barcodeLog.DepartmentId == departmentId
                             && letter.UrgencyLevel == EnumUrgentLevel.加急
                       select letter;

            var urg2 = from barcodeLog in _barcodeLogRepository.GetQueryable()
                       join letter in _letterRepository.GetQueryable() on barcodeLog.BarcodeNumber equals letter.BarcodeNo
                       where barcodeLog.OperationTime.Date >= beginDate.Date && barcodeLog.OperationTime <= endDate.Date &&
                             barcodeLog.DepartmentId == departmentId
                             && letter.UrgencyLevel == EnumUrgentLevel.特急
                       select letter;

            var urg3 = from barcodeLog in _barcodeLogRepository.GetQueryable()
                       join letter in _letterRepository.GetQueryable() on barcodeLog.BarcodeNumber equals letter.BarcodeNo
                       where barcodeLog.OperationTime.Date >= beginDate.Date && barcodeLog.OperationTime <= endDate.Date &&
                             barcodeLog.DepartmentId == departmentId
                             && letter.UrgencyLevel == EnumUrgentLevel.限时
                       select letter;

            var departmentStatistics = new DepartmentStatisticsDto
            {
                Receive = await receive.CountAsync(),
                ReceiveOut = await receiveOut.CountAsync(),
                Send = await send.CountAsync(),
                SendOut = await sendOut.CountAsync(),
                Sec0 = await sec0.CountAsync(),
                Sec1 = await sec1.CountAsync(),
                Sec2 = await sec2.CountAsync(),
                Sec3 = await sec3.CountAsync(),
                Urg0 = await urg0.CountAsync(),
                Urg1 = await urg1.CountAsync(),
                Urg2 = await urg2.CountAsync(),
                Urg3 = await urg3.CountAsync()
            };
            return departmentStatistics;
        }

        public async Task<PlaceStatisticsDto> GetPlaceStatistics(int placeId, DateTime beginDate, DateTime endDate)
        {

            var place = await _placeRepository.GetByIdAsync(placeId);

            var receive = from barcodeLog in _barcodeLogRepository.GetQueryable()
                          join letter in _letterRepository.GetQueryable() on barcodeLog.BarcodeNumber equals letter.BarcodeNo
                          where barcodeLog.BarcodeStatus == EnumBarcodeStatus.已签收 &&
                                barcodeLog.OperationTime.Date >= beginDate.Date && barcodeLog.OperationTime <= endDate.Date &&
                                barcodeLog.CurrentPlaceId == placeId
                          select letter;

            var receiveOut = from barcodeLog in _barcodeLogRepository.GetQueryable()
                             join letter in _letterRepository.GetQueryable() on barcodeLog.BarcodeNumber equals letter.BarcodeNo
                             where barcodeLog.BarcodeStatus == EnumBarcodeStatus.已签收 &&
                                   barcodeLog.OperationTime.Date >= beginDate.Date && barcodeLog.OperationTime <= endDate.Date &&
                                   barcodeLog.CurrentPlaceId == placeId
                                   && letter.LetterType == EnumLetterType.收信
                             select letter;

            var send = from barcodeLog in _barcodeLogRepository.GetQueryable()
                       join letter in _letterRepository.GetQueryable() on barcodeLog.BarcodeNumber equals letter.BarcodeNo
                       where barcodeLog.BarcodeStatus == EnumBarcodeStatus.已投递 &&
                             barcodeLog.OperationTime.Date >= beginDate.Date && barcodeLog.OperationTime <= endDate.Date &&
                             barcodeLog.CurrentPlaceId == placeId
                       select letter;

            var sendOut = from barcodeLog in _barcodeLogRepository.GetQueryable()
                          join letter in _letterRepository.GetQueryable() on barcodeLog.BarcodeNumber equals letter.BarcodeNo
                          where barcodeLog.BarcodeStatus == EnumBarcodeStatus.已投递 &&
                                barcodeLog.OperationTime.Date >= beginDate.Date && barcodeLog.OperationTime <= endDate.Date &&
                                barcodeLog.CurrentPlaceId == placeId
                                && letter.LetterType == EnumLetterType.发信
                          select letter;

            var sec0 = from barcodeLog in _barcodeLogRepository.GetQueryable()
                       join letter in _letterRepository.GetQueryable() on barcodeLog.BarcodeNumber equals letter.BarcodeNo
                       where barcodeLog.OperationTime.Date >= beginDate.Date && barcodeLog.OperationTime <= endDate.Date &&
                             barcodeLog.CurrentPlaceId == placeId
                             && letter.SecretLevel == EnumSecretLevel.无
                       select letter;

            var sec1 = from barcodeLog in _barcodeLogRepository.GetQueryable()
                       join letter in _letterRepository.GetQueryable() on barcodeLog.BarcodeNumber equals letter.BarcodeNo
                       where barcodeLog.OperationTime.Date >= beginDate.Date && barcodeLog.OperationTime <= endDate.Date &&
                             barcodeLog.CurrentPlaceId == placeId
                             && letter.SecretLevel == EnumSecretLevel.秘密
                       select letter;

            var sec2 = from barcodeLog in _barcodeLogRepository.GetQueryable()
                       join letter in _letterRepository.GetQueryable() on barcodeLog.BarcodeNumber equals letter.BarcodeNo
                       where barcodeLog.OperationTime.Date >= beginDate.Date && barcodeLog.OperationTime <= endDate.Date &&
                             barcodeLog.CurrentPlaceId == placeId
                             && letter.SecretLevel == EnumSecretLevel.机密
                       select letter;

            var sec3 = from barcodeLog in _barcodeLogRepository.GetQueryable()
                       join letter in _letterRepository.GetQueryable() on barcodeLog.BarcodeNumber equals letter.BarcodeNo
                       where barcodeLog.OperationTime.Date >= beginDate.Date && barcodeLog.OperationTime <= endDate.Date &&
                             barcodeLog.CurrentPlaceId == placeId
                             && letter.SecretLevel == EnumSecretLevel.绝密
                       select letter;

            var urg0 = from barcodeLog in _barcodeLogRepository.GetQueryable()
                       join letter in _letterRepository.GetQueryable() on barcodeLog.BarcodeNumber equals letter.BarcodeNo
                       where barcodeLog.OperationTime.Date >= beginDate.Date && barcodeLog.OperationTime <= endDate.Date &&
                             barcodeLog.CurrentPlaceId == placeId
                             && letter.UrgencyLevel == EnumUrgentLevel.无
                       select letter;

            var urg1 = from barcodeLog in _barcodeLogRepository.GetQueryable()
                       join letter in _letterRepository.GetQueryable() on barcodeLog.BarcodeNumber equals letter.BarcodeNo
                       where barcodeLog.OperationTime.Date >= beginDate.Date && barcodeLog.OperationTime <= endDate.Date &&
                             barcodeLog.CurrentPlaceId == placeId
                             && letter.UrgencyLevel == EnumUrgentLevel.加急
                       select letter;

            var urg2 = from barcodeLog in _barcodeLogRepository.GetQueryable()
                       join letter in _letterRepository.GetQueryable() on barcodeLog.BarcodeNumber equals letter.BarcodeNo
                       where barcodeLog.OperationTime.Date >= beginDate.Date && barcodeLog.OperationTime <= endDate.Date &&
                             barcodeLog.CurrentPlaceId == placeId
                             && letter.UrgencyLevel == EnumUrgentLevel.特急
                       select letter;

            var urg3 = from barcodeLog in _barcodeLogRepository.GetQueryable()
                       join letter in _letterRepository.GetQueryable() on barcodeLog.BarcodeNumber equals letter.BarcodeNo
                       where barcodeLog.OperationTime.Date >= beginDate.Date && barcodeLog.OperationTime <= endDate.Date &&
                             barcodeLog.CurrentPlaceId == placeId
                             && letter.UrgencyLevel == EnumUrgentLevel.限时
                       select letter;

            var placeStatistics = new PlaceStatisticsDto
            {
                PlaceId = placeId,
                PlaceName = place.Name,
                Receive = await receive.CountAsync(),
                ReceiveOut = await receiveOut.CountAsync(),
                Send = await send.CountAsync(),
                SendOut = await sendOut.CountAsync(),
                Sec0 = await sec0.CountAsync(),
                Sec1 = await sec1.CountAsync(),
                Sec2 = await sec2.CountAsync(),
                Sec3 = await sec3.CountAsync(),
                Urg0 = await urg0.CountAsync(),
                Urg1 = await urg1.CountAsync(),
                Urg2 = await urg2.CountAsync(),
                Urg3 = await urg3.CountAsync()
            };
            return placeStatistics;
        }

    }
}
