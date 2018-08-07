using System;
using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Core.Exchanges.Barcodes;
using BugChang.DES.Core.Exchanges.Bill;
using BugChang.DES.Core.Letters;
using BugChang.DES.Core.SerialNumbers;
using BugChang.DES.EntityFrameWorkCore;
using Microsoft.EntityFrameworkCore;

namespace BugChang.DES.Application.Bills
{
    public class BillAppService : IBillAppService
    {
        private readonly IBarcodeLogRepository _barcodeLogRepository;
        private readonly SerialNumberManager _serialNumberManager;
        private readonly ILetterRepository _letterRepository;
        private readonly IExchangeListRepository _exchangeListRepository;
        private readonly IExchangeListDetailRepository _exchangeListDetailRepository;
        private readonly UnitOfWork _unitOfWork;

        public BillAppService(IBarcodeLogRepository barcodeLogRepository, SerialNumberManager serialNumberManager, ILetterRepository letterRepository, IExchangeListDetailRepository exchangeListDetailRepository, IExchangeListRepository exchangeListRepository, UnitOfWork unitOfWork)
        {
            _barcodeLogRepository = barcodeLogRepository;
            _serialNumberManager = serialNumberManager;
            _letterRepository = letterRepository;
            _exchangeListDetailRepository = exchangeListDetailRepository;
            _exchangeListRepository = exchangeListRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<int> CreateReceiveBill(int placeId, int objectId, int userId, int departmentId)
        {
            var barcodeLogs = await _barcodeLogRepository.GetQueryable().Where(a =>
                !a.IsSynBill && a.CurrentPlaceId == placeId && a.CurrentObjectId == objectId &&
                a.BarcodeStatus == EnumBarcodeStatus.已签收).ToListAsync();
            var letters = _letterRepository.GetQueryable()
                .Where(a => barcodeLogs.Any(b => b.BarcodeNumber == a.BarcodeNo));
            //清单全局使用一个流水，防止串号
            var serialNo = await _serialNumberManager.GetSerialNumber(0, EnumSerialNumberType.清单);
            var exchangeList = new ExchangeList
            {
                CreateBy = userId,
                CreateTime = DateTime.Now,
                DepartmentId = departmentId,
                ObjectId = objectId,
                Printed = false,
                Type = EnumListType.收件清单
            };
            exchangeList.ListNo = exchangeList.GetListNo(serialNo);
            await _exchangeListRepository.AddAsync(exchangeList);

            foreach (var letter in letters)
            {
                var barcodeLog = barcodeLogs.FirstOrDefault(a => a.BarcodeNumber == letter.BarcodeNo);
                if (barcodeLog != null)
                {
                    var exchangeListDetail = new ExchangeListDetail
                    {
                        BarcodeNo = letter.BarcodeNo,
                        CustomData = letter.CustomData,
                        DetailType = barcodeLog.BarcodeSubStatus == EnumBarcodeSubStatus.退回 ? EnumListDetailType.收退件 : EnumListDetailType.收件,
                        ExchangeListId = exchangeList.Id,
                        ReceiveDepartmentName = letter.ReceiveDepartment.FullName,
                        SendDepartmentName = letter.SendDepartment.FullName,
                        SecSecretLevelText = letter.GetSecretLevel(letter.BarcodeNo).ToString(),
                        UrgencyLevelText = letter.GetUrgencyLevel(letter.BarcodeNo).ToString(),
                        Time = barcodeLog.LastOperationTime
                    };
                    await _exchangeListDetailRepository.AddAsync(exchangeListDetail);
                }
            }

            await _unitOfWork.CommitAsync();
            return exchangeList.Id;
        }
    }
}
