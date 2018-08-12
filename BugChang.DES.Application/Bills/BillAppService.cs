using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BugChang.DES.Application.Bills.Dtos;
using BugChang.DES.Core.Authorization.Users;
using BugChang.DES.Core.Commons;
using BugChang.DES.Core.Departments;
using BugChang.DES.Core.Exchanges.Barcodes;
using BugChang.DES.Core.Exchanges.Bill;
using BugChang.DES.Core.Exchanges.ExchangeObjects;
using BugChang.DES.Core.Exchanges.Places;
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
        private readonly IExchangeObjectRepository _exchangeObjectRepository;
        private readonly IUserRepository _userRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IPlaceWardenRepository _placeWardenRepository;
        private readonly UnitOfWork _unitOfWork;

        public BillAppService(IBarcodeLogRepository barcodeLogRepository, SerialNumberManager serialNumberManager, ILetterRepository letterRepository, IExchangeListDetailRepository exchangeListDetailRepository, IExchangeListRepository exchangeListRepository, UnitOfWork unitOfWork, IExchangeObjectRepository exchangeObjectRepository, IUserRepository userRepository, IDepartmentRepository departmentRepository, IPlaceWardenRepository placeWardenRepository)
        {
            _barcodeLogRepository = barcodeLogRepository;
            _serialNumberManager = serialNumberManager;
            _letterRepository = letterRepository;
            _exchangeListDetailRepository = exchangeListDetailRepository;
            _exchangeListRepository = exchangeListRepository;
            _unitOfWork = unitOfWork;
            _exchangeObjectRepository = exchangeObjectRepository;
            _userRepository = userRepository;
            _departmentRepository = departmentRepository;
            _placeWardenRepository = placeWardenRepository;
        }

        /// <summary>
        /// 生成收件清单
        /// </summary>
        /// <param name="placeId">场所ID</param>
        /// <param name="objectId">流转对象ID</param>
        /// <param name="userId">用户ID</param>
        /// <param name="departmentId">收件单位ID</param>
        /// <returns></returns>
        public async Task<ResultEntity> CreateReceiveBill(int placeId, int objectId, int userId, int departmentId)
        {
            var result = new ResultEntity();
            var barcodeLogs = await _barcodeLogRepository.GetQueryable().Where(a =>
                !a.IsSynBill && a.CurrentPlaceId == placeId && a.CurrentObjectId == objectId &&
                a.BarcodeStatus == EnumBarcodeStatus.已签收).ToListAsync();
            if (barcodeLogs.Count > 0)
            {
                var exchangeObject = await _exchangeObjectRepository.GetByIdAsync(objectId);
                var user = await _userRepository.GetByIdAsync(userId);
                var letters = _letterRepository.GetQueryable()
                    .Where(a => barcodeLogs.Any(b => b.BarcodeNumber == a.BarcodeNo));
                //清单全局使用一个流水，防止串号
                var serialNo = await _serialNumberManager.GetSerialNumber(0, EnumSerialNumberType.清单);
                var exchangeList = new ExchangeList
                {
                    CreateBy = userId,
                    CreateTime = DateTime.Now,
                    DepartmentId = departmentId,
                    ObjectName = exchangeObject.Name,
                    ExchangeUserId = userId,
                    ExchangeUserName = user.DisplayName,
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
                        result.Success = true;
                        result.Data = exchangeList.Id;
                    }
                }

                await _unitOfWork.CommitAsync();
            }
            else
            {
                result.Message = "暂无收件清单";
            }

            return result;
        }

        /// <summary>
        /// 生成发件清单
        /// </summary>
        /// <param name="placeId">场所ID</param>
        /// <param name="userId">用户ID</param>
        /// <param name="departmentId">发件单位ID</param>
        /// <returns></returns>
        public async Task<ResultEntity> CreateSendBill(int placeId, int userId, int departmentId)
        {
            var result = new ResultEntity();
            var barcodeLogs = await _barcodeLogRepository.GetQueryable().Where(a =>
                !a.IsSynBill && a.CurrentPlaceId == placeId && a.DepartmentId == departmentId &&
                a.BarcodeStatus == EnumBarcodeStatus.已签收).ToListAsync();
            if (barcodeLogs.Count > 0)
            {
                var department = await _departmentRepository.GetByIdAsync(departmentId);
                var user = await _userRepository.GetByIdAsync(userId);
                var letters = _letterRepository.GetQueryable()
                    .Where(a => barcodeLogs.Any(b => b.BarcodeNumber == a.BarcodeNo));
                //清单全局使用一个流水，防止串号
                var serialNo = await _serialNumberManager.GetSerialNumber(0, EnumSerialNumberType.清单);
                var exchangeList = new ExchangeList
                {
                    CreateBy = userId,
                    CreateTime = DateTime.Now,
                    DepartmentId = departmentId,
                    ObjectName = department.Name,
                    ExchangeUserId = userId,
                    ExchangeUserName = user.DisplayName,
                    Printed = false,
                    Type = EnumListType.发件清单
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
                            DetailType = barcodeLog.BarcodeSubStatus == EnumBarcodeSubStatus.退回 ? EnumListDetailType.发退件 : EnumListDetailType.发件,
                            ExchangeListId = exchangeList.Id,
                            ReceiveDepartmentName = letter.ReceiveDepartment.FullName,
                            SendDepartmentName = letter.SendDepartment.FullName,
                            SecSecretLevelText = letter.GetSecretLevel(letter.BarcodeNo).ToString(),
                            UrgencyLevelText = letter.GetUrgencyLevel(letter.BarcodeNo).ToString(),
                            Time = barcodeLog.OperationTime
                        };
                        await _exchangeListDetailRepository.AddAsync(exchangeListDetail);
                    }
                }
                await _unitOfWork.CommitAsync();
                result.Data = exchangeList.Id;
                result.Success = true;
            }
            else
            {
                result.Message = "暂无发件清单";
            }

            return result;
        }

        /// <summary>
        /// 生成收发清单
        /// </summary>
        /// <param name="placeId">场所ID</param>
        /// <param name="userId">用户ID</param>
        /// <param name="departmentId">单位ID</param>
        /// <returns></returns>
        public async Task<ResultEntity> CreateReceiveSendBill(int placeId, int userId, int departmentId)
        {
            var result = new ResultEntity();

            var receiveBarcodeLogs = await _barcodeLogRepository.GetQueryable().Where(a =>
                !a.IsSynBill && a.CurrentPlaceId == placeId && a.DepartmentId == departmentId &&
                a.BarcodeStatus == EnumBarcodeStatus.已签收).ToListAsync();
            var sendBarcodeLogs = await _barcodeLogRepository.GetQueryable().Where(a =>
                !a.IsSynBill && a.CurrentPlaceId == placeId && a.DepartmentId == departmentId &&
                a.BarcodeStatus == EnumBarcodeStatus.已投递).ToListAsync();
            if (receiveBarcodeLogs.Count > 0 || sendBarcodeLogs.Count > 0)
            {
                //清单全局使用一个流水，防止串号
                var serialNo = await _serialNumberManager.GetSerialNumber(0, EnumSerialNumberType.清单);
                var receiveLettesr = _letterRepository.GetQueryable()
                    .Where(a => receiveBarcodeLogs.Any(b => b.BarcodeNumber == a.BarcodeNo));
                var sendLettesr = _letterRepository.GetQueryable()
                    .Where(a => sendBarcodeLogs.Any(b => b.BarcodeNumber == a.BarcodeNo));
                var department = await _departmentRepository.GetByIdAsync(departmentId);
                var user = await _userRepository.GetByIdAsync(userId);
                //添加主清单
                var exchangeList = new ExchangeList
                {
                    CreateBy = userId,
                    ObjectName = department.Name,
                    ExchangeUserId = userId,
                    ExchangeUserName = user.DisplayName,
                    CreateTime = DateTime.Now,
                    DepartmentId = departmentId,
                    Printed = false,
                    Type = EnumListType.收发清单
                };
                exchangeList.ListNo = exchangeList.GetListNo(serialNo);
                await _exchangeListRepository.AddAsync(exchangeList);

                //添加收件详情
                foreach (var letter in receiveLettesr)
                {
                    var barcodeLog = receiveBarcodeLogs.FirstOrDefault(a => a.BarcodeNumber == letter.BarcodeNo);
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
                //添加发件详情
                foreach (var letter in sendLettesr)
                {
                    var barcodeLog = receiveBarcodeLogs.FirstOrDefault(a => a.BarcodeNumber == letter.BarcodeNo);
                    if (barcodeLog != null)
                    {
                        var exchangeListDetail = new ExchangeListDetail
                        {
                            BarcodeNo = letter.BarcodeNo,
                            CustomData = letter.CustomData,
                            DetailType = barcodeLog.BarcodeSubStatus == EnumBarcodeSubStatus.退回 ? EnumListDetailType.发退件 : EnumListDetailType.发件,
                            ExchangeListId = exchangeList.Id,
                            ReceiveDepartmentName = letter.ReceiveDepartment.FullName,
                            SendDepartmentName = letter.SendDepartment.FullName,
                            SecSecretLevelText = letter.GetSecretLevel(letter.BarcodeNo).ToString(),
                            UrgencyLevelText = letter.GetUrgencyLevel(letter.BarcodeNo).ToString(),
                            Time = barcodeLog.OperationTime
                        };
                        await _exchangeListDetailRepository.AddAsync(exchangeListDetail);
                    }
                }

                await _unitOfWork.CommitAsync();
                result.Success = true;
                result.Data = exchangeList.Id;
            }
            else
            {
                result.Message = "暂无收发清单数据";
            }

            return result;
        }

        public async Task<BillDto> GetBill(int id)
        {
            var bill = await _exchangeListRepository.GetByIdAsync(id);
            return Mapper.Map<BillDto>(bill);
        }

        public async Task<IList<BillDetailDto>> GetBillDetails(int billId)
        {
            var billDetails = await _exchangeListDetailRepository.GetQueryable().Where(a => a.ExchangeListId == billId)
                .ToListAsync();
            return Mapper.Map<IList<BillDetailDto>>(billDetails);
        }

        public async Task<PageResultModel<BillDto>> GetBills(PageSearchCommonModel pageSearch)
        {
            var departmentIds = new List<int> { pageSearch.DepartmentId };
            var warden = await _placeWardenRepository.GetQueryable().Include(a => a.Place).FirstOrDefaultAsync(a => a.UserId == pageSearch.UserId);
            if (warden != null)
            {
                departmentIds.Add(warden.Place.DepartmentId);
            }

            var queryable = _exchangeListRepository.GetQueryable().Where(a => departmentIds.Contains(a.DepartmentId));
            if (!string.IsNullOrWhiteSpace(pageSearch.Keywords))
            {
                queryable = queryable.Where(a =>
                    a.ListNo.Contains(pageSearch.Keywords) || a.ObjectName.Contains(pageSearch.Keywords) ||
                    a.ExchangeUserName.Contains(pageSearch.Keywords));
            }
            var pageResult = new PageResultModel<ExchangeList>
            {
                Rows = await queryable.OrderByDescending(a => a.Id).Skip(pageSearch.Skip).Take(pageSearch.Take).ToListAsync(),
                Total = await queryable.CountAsync()
            };
            return Mapper.Map<PageResultModel<BillDto>>(pageResult);
        }
    }
}
