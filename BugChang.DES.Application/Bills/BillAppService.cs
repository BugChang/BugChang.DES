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
using Microsoft.Extensions.Logging;

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
        private readonly ILogger<BillAppService> _logger;

        public BillAppService(IBarcodeLogRepository barcodeLogRepository, SerialNumberManager serialNumberManager, ILetterRepository letterRepository, IExchangeListDetailRepository exchangeListDetailRepository, IExchangeListRepository exchangeListRepository, UnitOfWork unitOfWork, IExchangeObjectRepository exchangeObjectRepository, IUserRepository userRepository, IDepartmentRepository departmentRepository, IPlaceWardenRepository placeWardenRepository, ILogger<BillAppService> logger)
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
            _logger = logger;
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
            _logger.LogWarning($"--------------开始形成取件清单--------------");
            _logger.LogWarning($"placeId：{placeId},objectId:{objectId},userId:{userId},departmentId:{departmentId}");
            var result = new ResultEntity();
            var barcodeLogs = await _barcodeLogRepository.GetQueryable().Where(a =>
                !a.IsSynBill && a.CurrentPlaceId == placeId && a.CurrentObjectId == objectId &&
                a.BarcodeStatus == EnumBarcodeStatus.已签收).ToListAsync();
            _logger.LogWarning($"流转记录条数：{barcodeLogs.Count}");
            if (barcodeLogs.Count > 0)
            {
                var exchangeObject = await _exchangeObjectRepository.GetByIdAsync(objectId);
                var user = await _userRepository.GetByIdAsync(userId);
                var letters = await _letterRepository.GetQueryable().Include(a => a.ReceiveDepartment).Include(a => a.SendDepartment)
                    .Where(a => barcodeLogs.Exists(b => b.BarcodeNumber == a.BarcodeNo)).ToListAsync();
                //_logger.LogWarning($"信件记录条数：{letters.Count()}");
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
                await _unitOfWork.CommitAsync();
                foreach (var letter in letters)
                {
                    var barcodeLog = barcodeLogs.FirstOrDefault(a => a.BarcodeNumber == letter.BarcodeNo);
                    if (barcodeLog != null)
                    {
                        var exchangeListDetail = new ExchangeListDetail
                        {
                            BarcodeNo = letter.LetterNo,
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
                        barcodeLog.IsSynBill = true;
                    }
                }
                result.Success = true;
                result.Data = exchangeList.Id;
                await _unitOfWork.CommitAsync();
            }
            else
            {
                _logger.LogWarning($"暂无收件清单");
                result.Message = "暂无收件清单";
            }
            _logger.LogWarning($"--------------结束形成取件清单--------------");
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
            _logger.LogWarning($"--------------开始形成取件清单--------------");
            //_logger.LogWarning($"placeId：{placeId},userId:{userId},departmentId:{departmentId}");
            var result = new ResultEntity();
            var barcodeLogs = await _barcodeLogRepository.GetQueryable().Where(a =>
                !a.IsSynBill && a.CurrentPlaceId == placeId && a.DepartmentId == departmentId &&
                a.BarcodeStatus == EnumBarcodeStatus.已投递).ToListAsync();
            //_logger.LogWarning($"流转记录条数：{barcodeLogs.Count}");
            if (barcodeLogs.Count > 0)
            {
                var department = await _departmentRepository.GetByIdAsync(departmentId);
                var user = await _userRepository.GetByIdAsync(userId);
                var letters = await _letterRepository.GetQueryable().Include(a => a.ReceiveDepartment).Include(a => a.SendDepartment)
                    .Where(a => barcodeLogs.Exists(b => b.BarcodeNumber == a.BarcodeNo)).ToListAsync();
                // _logger.LogWarning($"信件记录条数：{letters.Count()}");
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
                await _unitOfWork.CommitAsync();
                foreach (var letter in letters)
                {
                    var barcodeLog = barcodeLogs.FirstOrDefault(a => a.BarcodeNumber == letter.BarcodeNo);
                    if (barcodeLog != null)
                    {
                        var exchangeListDetail = new ExchangeListDetail
                        {
                            BarcodeNo = letter.LetterNo,
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
                        barcodeLog.IsSynBill = true;

                    }
                }
                await _unitOfWork.CommitAsync();
                result.Data = exchangeList.Id;
                result.Success = true;

            }
            else
            {
                result.Message = "暂无发件清单";
                _logger.LogWarning($"暂无收件清单");
            }
            _logger.LogWarning($"--------------结束形成发件清单--------------");
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
            _logger.LogWarning($"--------------开始形成收发件清单--------------");
            var result = new ResultEntity();
            var receiveBarcodeLogs = await _barcodeLogRepository.GetQueryable().Where(a =>
                !a.IsSynBill && a.CurrentPlaceId == placeId && a.DepartmentId == departmentId &&
                a.BarcodeStatus == EnumBarcodeStatus.已签收).ToListAsync();
            _logger.LogWarning($"收件流转记录条数：{receiveBarcodeLogs.Count}");
            var sendBarcodeLogs = await _barcodeLogRepository.GetQueryable().Where(a =>
                !a.IsSynBill && a.CurrentPlaceId == placeId && a.DepartmentId == departmentId &&
                a.BarcodeStatus == EnumBarcodeStatus.已投递).ToListAsync();
            _logger.LogWarning($"发件流转记录条数：{sendBarcodeLogs.Count}");
            if (receiveBarcodeLogs.Count > 0 || sendBarcodeLogs.Count > 0)
            {
                //清单全局使用一个流水，防止串号
                var serialNo = await _serialNumberManager.GetSerialNumber(0, EnumSerialNumberType.清单);
                var receiveLetters = await _letterRepository.GetQueryable().Include(a => a.ReceiveDepartment).Include(a => a.SendDepartment)
                    .Where(a => receiveBarcodeLogs.Exists(b => b.BarcodeNumber == a.BarcodeNo)).ToListAsync();
                //_logger.LogWarning($"收信记录条数：{receiveLettesr.Count()}");
                var sendLetters = await _letterRepository.GetQueryable().Include(a => a.ReceiveDepartment).Include(a => a.SendDepartment)
                    .Where(a => sendBarcodeLogs.Exists(b => b.BarcodeNumber == a.BarcodeNo)).ToListAsync();
                // _logger.LogWarning($"发信记录条数：{receiveLettesr.Count()}");
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
                await _unitOfWork.CommitAsync();
                //添加收件详情
                foreach (var letter in receiveLetters)
                {
                    var barcodeLog = receiveBarcodeLogs.FirstOrDefault(a => a.BarcodeNumber == letter.BarcodeNo);
                    if (barcodeLog != null)
                    {
                        var exchangeListDetail = new ExchangeListDetail
                        {
                            BarcodeNo = letter.LetterNo,
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
                        barcodeLog.IsSynBill = true;
                    }
                }
                //添加发件详情
                foreach (var letter in sendLetters)
                {
                    var barcodeLog = sendBarcodeLogs.FirstOrDefault(a => a.BarcodeNumber == letter.BarcodeNo);
                    if (barcodeLog != null)
                    {
                        var exchangeListDetail = new ExchangeListDetail
                        {
                            BarcodeNo = letter.LetterNo,
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
                        barcodeLog.IsSynBill = true;
                    }
                }

                await _unitOfWork.CommitAsync();
                result.Success = true;
                result.Data = exchangeList.Id;
            }
            else
            {
                result.Message = "暂无收发清单数据";
                _logger.LogWarning($"暂无收发清单数据");
            }
            _logger.LogWarning($"--------------结束形成收发件清单--------------");
            return result;
        }

        public async Task<BillDto> GetBill(int id)
        {
            var bill = await _exchangeListRepository.GetQueryable().Include(a => a.CreateUser).Include(a => a.UpdateUser).FirstOrDefaultAsync(a => a.Id == id);
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

            var queryable = _exchangeListRepository.GetQueryable().Include(a => a.CreateUser).Where(a => departmentIds.Contains(a.DepartmentId));
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
