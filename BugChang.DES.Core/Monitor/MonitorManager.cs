using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Core.Authentication.Card;
using BugChang.DES.Core.Authorization.Users;
using BugChang.DES.Core.Departments;
using BugChang.DES.Core.Exchanges.Barcodes;
using BugChang.DES.Core.Exchanges.Boxs;
using BugChang.DES.Core.Exchanges.Channel;
using BugChang.DES.Core.Exchanges.ExchangeObjects;
using BugChang.DES.Core.Exchanges.Places;
using BugChang.DES.Core.Letters;
using BugChang.DES.Core.Logs;
using BugChang.DES.Core.SecretLevels;
using BugChang.DES.Core.Sortings;
using Microsoft.EntityFrameworkCore;

namespace BugChang.DES.Core.Monitor
{
    public class MonitorManager
    {
        private readonly IPlaceRepository _placeRepository;
        private readonly IBarcodeRepository _barcodeRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IBoxObjectRepository _boxObjectRepository;
        private readonly LogManager _logManager;
        private readonly BarcodeManager _barcodeManager;
        private readonly IBoxRepository _boxRepository;
        private readonly ICardRepository _cardRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPlaceWardenRepository _placeWardenRepository;
        private readonly IExchangeObjectSignerRepository _objectSignerRepository;
        private readonly ILetterRepository _letterRepository;
        private readonly IBarcodeLogRepository _barcodeLogRepository;
        private readonly IExchangeObjectRepository _objectRepository;
        private readonly ISortingRepository _sortingRepository;
        public MonitorManager(IPlaceRepository placeRepository, IBarcodeRepository barcodeRepository, IBoxObjectRepository boxObjectRepository,
            LogManager logManager, BarcodeManager barcodeManager, IBoxRepository boxRepository, ICardRepository cardRepository, IUserRepository userRepository, IPlaceWardenRepository placeWardenRepository,
            IExchangeObjectSignerRepository objectSignerRepository, ILetterRepository letterRepository, IBarcodeLogRepository barcodeLogRepository, IExchangeObjectRepository objectRepository,
            IDepartmentRepository departmentRepository, ISortingRepository sortingRepository)
        {
            _placeRepository = placeRepository;
            _barcodeRepository = barcodeRepository;
            _boxObjectRepository = boxObjectRepository;
            _logManager = logManager;
            _barcodeManager = barcodeManager;
            _boxRepository = boxRepository;
            _cardRepository = cardRepository;
            _userRepository = userRepository;
            _placeWardenRepository = placeWardenRepository;
            _objectSignerRepository = objectSignerRepository;
            _letterRepository = letterRepository;
            _barcodeLogRepository = barcodeLogRepository;
            _objectRepository = objectRepository;
            _departmentRepository = departmentRepository;
            _sortingRepository = sortingRepository;
        }

        public async Task<CheckBarcodeModel> CheckBarcodeType(string barcodeNo, int placeId)
        {

            var checkBarcodeModel = new CheckBarcodeModel();
            if (barcodeNo.Length == 33 || barcodeNo.Length == 26)
            {
                var barcode = await _barcodeRepository.GetByNoAsync(barcodeNo);
                var letter = await _letterRepository.GetLetter(barcodeNo) ?? new Letter
                {
                    ReceiveDepartmentId = await _barcodeManager.GetReceiveDepartmentId(barcodeNo),
                    SendDepartmentId = await _barcodeManager.GetSendDepartmentId(barcodeNo),
                    //如果数据库中没有Letter记录，那么LetterType一定是收信
                    LetterType = EnumLetterType.收信
                };
                if (barcode != null)
                {
                    switch (barcode.Status)
                    {
                        case EnumBarcodeStatus.已签收:
                            if (barcode.CurrentPlaceId == placeId && barcode.SubStatus == EnumBarcodeSubStatus.正常)
                            {
                                //同一场所下，已签收的文件禁止再次投箱（勘误、退回件除外）
                                checkBarcodeModel.Type = EnumCheckBarcodeType.无效;
                            }
                            else
                            {
                                return await CheckBarcodeTypeCommon(letter, placeId);
                            }
                            break;
                        case EnumBarcodeStatus.已就绪:
                        case EnumBarcodeStatus.已勘误:
                            return await CheckBarcodeTypeCommon(letter, placeId);
                        case EnumBarcodeStatus.已投递:
                            checkBarcodeModel.Type = EnumCheckBarcodeType.条码已经投箱;
                            break;
                        case EnumBarcodeStatus.申请退回:
                            var receiveDepartmentId = letter.ReceiveDepartmentId;
                            letter.ReceiveDepartmentId = letter.SendDepartmentId;
                            letter.SendDepartmentId = receiveDepartmentId;
                            return await CheckBarcodeTypeCommon(letter, placeId);
                        case EnumBarcodeStatus.已退回:
                            checkBarcodeModel.Type = EnumCheckBarcodeType.无效;
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                else
                {
                    return await CheckBarcodeTypeCommon(letter, placeId);
                }
            }

            return checkBarcodeModel;
        }


        private async Task<CheckBarcodeModel> CheckBarcodeTypeCommon(Letter letter, int placeId)
        {
            var checkBarcodeModel = new CheckBarcodeModel();
            switch (letter.LetterType)
            {
                case EnumLetterType.收信:
                    {
                        var exchangeObject = await _objectRepository.GetQueryable().Where(a =>
                               a.ObjectType == EnumObjectType.机构 && a.Value == letter.ReceiveDepartmentId)
                           .FirstOrDefaultAsync();

                        var tempObject = exchangeObject;
                        var boxs = await _boxObjectRepository.GetQueryable()
                            .Where(a => a.Box.PlaceId == placeId && a.ExchangeObjectId == tempObject.Id && a.Box.Enabled)
                            .Select(a => a.Box).ToListAsync();
                        if (boxs.Count > 0)
                        {
                            checkBarcodeModel.Type = EnumCheckBarcodeType.唯一指定;
                            checkBarcodeModel.Record =
                                boxs.Select(a => new CheckedBarcodeRecord { FileCount = 1, Message = "", No = a.Id })
                                    .ToList();
                        }
                        else
                        {
                            while (exchangeObject.ParentId != null)
                            {
                                var o = exchangeObject;
                                exchangeObject = await _objectRepository.GetQueryable().Where(a => a.Id == o.ParentId.Value)
                                    .FirstOrDefaultAsync();
                                var exchangeObject1 = exchangeObject;
                                boxs = await _boxObjectRepository.GetQueryable()
                                    .Where(a => a.Box.PlaceId == placeId && a.ExchangeObjectId == exchangeObject1.Id && a.Box.Enabled)
                                    .Select(a => a.Box).ToListAsync();
                                if (boxs.Count > 0)
                                {
                                    checkBarcodeModel.Type = EnumCheckBarcodeType.唯一指定;
                                    checkBarcodeModel.Record = boxs.Select(a =>
                                        new CheckedBarcodeRecord { FileCount = 1, Message = "", No = a.Id }).ToList();
                                    break;
                                }
                            }
                            checkBarcodeModel.Type = EnumCheckBarcodeType.无效;
                        }
                    }
                    break;
                case EnumLetterType.发信:
                    {
                        var receiveDepartment = await _departmentRepository.GetByIdAsync(letter.ReceiveDepartmentId);
                        var receiveChannel = receiveDepartment.ReceiveChannel;
                        if (letter.BarcodeNo.Length == 26 && letter.SecretLevel == EnumSecretLevel.绝密)
                        {
                            //特殊逻辑，交换站绝密件走市机
                            receiveChannel = EnumChannel.机要通信;
                        }
                        var channelExchangeObjects = await _objectRepository.GetQueryable().Where(a =>
                                a.ObjectType == EnumObjectType.渠道 && a.Value == (int)receiveChannel)
                            .ToListAsync();
                        if (channelExchangeObjects.Count > 0)
                        {
                            var boxs = await _boxObjectRepository.GetQueryable()
                                .Where(a => a.Box.PlaceId == placeId && channelExchangeObjects.Any(b => b.Id == a.ExchangeObjectId) && a.Box.Enabled)
                                .Select(a => a.Box).ToListAsync();
                            if (boxs.Count > 0)
                            {
                                checkBarcodeModel.Type = EnumCheckBarcodeType.唯一指定;
                                checkBarcodeModel.Record = boxs.Select(a =>
                                    new CheckedBarcodeRecord { FileCount = 1, Message = "", No = a.Id }).ToList();
                            }
                            else
                            {
                                checkBarcodeModel.Type = EnumCheckBarcodeType.无效;
                            }
                        }
                        else
                        {
                            var currentSend = false;
                            //判断是否是该场所下的单位发件
                            var place = await _placeRepository.GetByIdAsync(placeId);
                            var sendDepartment = await _departmentRepository.GetByIdAsync(letter.SendDepartmentId);
                            while (sendDepartment.ParentId != null)
                            {
                                if (place.DepartmentId == sendDepartment.Id)
                                {
                                    currentSend = true;
                                }
                                else
                                {
                                    sendDepartment = await _departmentRepository.GetByIdAsync(sendDepartment.ParentId.Value);
                                }
                            }

                            if (currentSend)
                            {
                                //当前场所下单位发件
                                var insideExchangeObjects = await _objectRepository.GetQueryable().Where(a =>
                                        a.ObjectType == EnumObjectType.渠道 && a.Value == (int)EnumChannel.内部)
                                    .ToListAsync();
                                var receiveCode = letter.GetReceiveCode(letter.BarcodeNo);
                                var matchExchangeObjects = insideExchangeObjects.Where(a => receiveCode.Contains(a.RestrictionCode)).ToList();
                                var boxs = await _boxObjectRepository.GetQueryable()
                                    .Where(a => a.Box.PlaceId == placeId && insideExchangeObjects.Any(b => b.Id == a.ExchangeObjectId) && a.Box.Enabled)
                                    .Select(a => a.Box).ToListAsync();
                                if (matchExchangeObjects.Count > 0)
                                {
                                    boxs = await _boxObjectRepository.GetQueryable()
                                        .Where(a => a.Box.PlaceId == placeId && matchExchangeObjects.Any(b => b.Id == a.ExchangeObjectId) && a.Box.Enabled)
                                        .Select(a => a.Box).ToListAsync();
                                }
                                checkBarcodeModel.Type = EnumCheckBarcodeType.唯一指定;
                                checkBarcodeModel.Record = boxs.Select(a =>
                                    new CheckedBarcodeRecord { FileCount = 1, Message = "", No = a.Id }).ToList();
                            }
                            else
                            {
                                checkBarcodeModel.Type = EnumCheckBarcodeType.无效;
                            }
                        }
                    }
                    break;
                case EnumLetterType.内交换:
                    {
                        //收件单位在这个交换场所下有没有箱子，有的话直接返回

                        //没有的话循环判断父流转对象在这有没有箱子，有的话返回，没有的话判断是不是这个场所下的单位发件

                        //是的话，按照限制码找最佳匹配的内部渠道箱子,不是直接返回无效

                        {
                            var exchangeObject = await _objectRepository.GetQueryable().Where(a =>
                                   a.ObjectType == EnumObjectType.机构 && a.Value == letter.ReceiveDepartmentId)
                               .FirstOrDefaultAsync();

                            var tempObject = exchangeObject;
                            var boxs = await _boxObjectRepository.GetQueryable()
                                .Where(a => a.Box.PlaceId == placeId && a.ExchangeObjectId == tempObject.Id && a.Box.Enabled)
                                .Select(a => a.Box).ToListAsync();
                            if (boxs.Count > 0)
                            {
                                //当前场所下存在收件流转对象箱格
                                checkBarcodeModel.Type = EnumCheckBarcodeType.唯一指定;
                                checkBarcodeModel.Record =
                                    boxs.Select(a => new CheckedBarcodeRecord { FileCount = 1, Message = "", No = a.Id })
                                        .ToList();
                            }
                            else
                            {
                                var isFind = false;
                                //循环判断上级流转对象在这有没有箱子
                                while (exchangeObject.ParentId != null)
                                {
                                    var o = exchangeObject;
                                    exchangeObject = await _objectRepository.GetQueryable().Where(a => a.Id == o.ParentId.Value)
                                        .FirstOrDefaultAsync();
                                    var exchangeObject1 = exchangeObject;
                                    boxs = await _boxObjectRepository.GetQueryable()
                                        .Where(a => a.Box.PlaceId == placeId && a.ExchangeObjectId == exchangeObject1.Id && a.Box.Enabled)
                                        .Select(a => a.Box).ToListAsync();
                                    if (boxs.Count > 0)
                                    {
                                        checkBarcodeModel.Type = EnumCheckBarcodeType.唯一指定;
                                        checkBarcodeModel.Record = boxs.Select(a =>
                                            new CheckedBarcodeRecord { FileCount = 1, Message = "", No = a.Id }).ToList();
                                        isFind = true;
                                        break;
                                    }
                                }

                                if (!isFind)
                                {
                                    var currentSend = false;
                                    //判断是否是该场所下的单位发件
                                    var place = await _placeRepository.GetByIdAsync(placeId);
                                    var sendDepartment = await _departmentRepository.GetByIdAsync(letter.SendDepartmentId);
                                    while (sendDepartment.ParentId != null)
                                    {
                                        if (place.DepartmentId == sendDepartment.Id)
                                        {
                                            currentSend = true;
                                        }
                                        else
                                        {
                                            sendDepartment = await _departmentRepository.GetByIdAsync(sendDepartment.ParentId.Value);
                                        }
                                    }

                                    if (currentSend)
                                    {
                                        //当前场所下单位发件
                                        var exchangeObjects = await _objectRepository.GetQueryable().Where(a =>
                                                a.ObjectType == EnumObjectType.渠道 && a.Value == (int)EnumChannel.内部)
                                            .ToListAsync();
                                        var receiveCode = letter.GetReceiveCode(letter.BarcodeNo);
                                        var matchExchangeObjects = exchangeObjects.Where(a => receiveCode.Contains(a.RestrictionCode)).ToList();
                                        if (matchExchangeObjects.Count > 0)
                                        {
                                            boxs = await _boxObjectRepository.GetQueryable()
                                                .Where(a => a.Box.PlaceId == placeId && matchExchangeObjects.Any(b => b.Id == a.ExchangeObjectId) && a.Box.Enabled)
                                                .Select(a => a.Box).ToListAsync();
                                        }
                                        else
                                        {
                                            boxs = await _boxObjectRepository.GetQueryable()
                                                .Where(a => a.Box.PlaceId == placeId && exchangeObjects.Any(b => b.Id == a.ExchangeObjectId) && a.Box.Enabled)
                                                .Select(a => a.Box).ToListAsync();
                                        }
                                        checkBarcodeModel.Type = EnumCheckBarcodeType.唯一指定;
                                        checkBarcodeModel.Record = boxs.Select(a =>
                                            new CheckedBarcodeRecord { FileCount = 1, Message = "", No = a.Id }).ToList();
                                    }
                                    else
                                    {
                                        checkBarcodeModel.Type = EnumCheckBarcodeType.无效;
                                    }
                                }
                            }
                        }
                    }
                    break;
            }

            return checkBarcodeModel;
        }

        public async Task<IList<Box>> GetAllBoxs(int placeId)
        {
            var boxs = await _boxRepository.GetQueryable().Where(a => a.PlaceId == placeId).ToListAsync();
            return boxs;
        }

        public async Task<Box> GetBox(int boxId)
        {
            var box = await _boxRepository.GetByIdAsync(boxId);
            return box;
        }

        public async Task<CheckCardTypeModel> CheckCardType(int placeId, int boxId, string cardValue)
        {

            var card = await _cardRepository.GetQueryable().Where(a => a.Value == cardValue).FirstOrDefaultAsync();
            var user = await _userRepository.GetQueryable().Include(a => a.Department).FirstOrDefaultAsync(a => a.Id == card.UserId);
            var objects = await _boxObjectRepository.GetQueryable().Where(a => a.BoxId == boxId).ToListAsync();
            var objectSigner = await _objectSignerRepository.GetQueryable().Where(a => objects.Any(b => b.ExchangeObjectId == a.ExchangeObjectId) && a.UserId == user.Id).FirstOrDefaultAsync();
            var checkCardModel = new CheckCardTypeModel
            {
                Boxs = new List<int>
                {
                    boxId
                },
                UserName = user.DisplayName,
                DepartmentName = user.Department.Name
            };
            if (objectSigner != null)
            {
                checkCardModel.Type = 22;
            }
            else
            {
                var placeWarden = await _placeWardenRepository.GetQueryable().Where(a => a.PlaceId == placeId && a.UserId == user.Id).FirstOrDefaultAsync();
                if (placeWarden != null)
                {
                    //当前刷卡人为本场所管理员
                    checkCardModel.Type = 21;
                }
            }

            return checkCardModel;
        }

        public async Task<int> SaveLetter(int placeId, string barCodeNo, int boxId, int fileCount, bool isJiaJi)
        {
            var place = await _placeRepository.GetByIdAsync(placeId);
            var letter = await _letterRepository.GetQueryable().Where(a => a.BarcodeNo == barCodeNo).FirstOrDefaultAsync();
            if (letter == null)
            {
                //保存信件基本信息
                letter = new Letter
                {
                    BarcodeNo = barCodeNo
                };
                letter.LetterNo = letter.GetLetterNo(barCodeNo);
                //本系统未登记过的信件，全部判定为收信
                letter.LetterType = EnumLetterType.收信;
                letter.ReceiveDepartmentId = await _barcodeManager.GetReceiveDepartmentId(barCodeNo);
                letter.SendDepartmentId = await _barcodeManager.GetSendDepartmentId(barCodeNo);
                letter.SecretLevel = letter.GetSecretLevel(barCodeNo);
                letter.UrgencyLevel = letter.GetUrgencyLevel(barCodeNo);
                letter.CreateTime = DateTime.Now;
                await _letterRepository.AddAsync(letter);
            }

            var barCode = await _barcodeRepository.GetQueryable().Where(a => a.BarcodeNo == barCodeNo)
                .FirstOrDefaultAsync();

            var boxObject = await _boxObjectRepository.GetQueryable().Where(a => a.BoxId == boxId).FirstOrDefaultAsync();

            if (barCode == null)
            {
                //添加条码记录
                barCode = new Barcode
                {
                    BarcodeNo = barCodeNo,
                    Entity = EnumBarcodeEntity.信件,
                    Souce = EnumBarcodeSouce.外部,
                    Status = EnumBarcodeStatus.已投递,
                    SubStatus = EnumBarcodeSubStatus.正常,
                    CreateTime = DateTime.Now,
                    CurrentObjectId = boxObject.ExchangeObjectId
                };
                barCode.BarcodeType = barCode.AnalysisBarcodeType(barCodeNo);
                await _barcodeRepository.AddAsync(barCode);
            }
            barCode.UpdateTime = DateTime.Now;
            switch (barCode.Status)
            {
                case EnumBarcodeStatus.已就绪:
                case EnumBarcodeStatus.已签收:
                case EnumBarcodeStatus.已勘误:
                    barCode.Status = EnumBarcodeStatus.已投递;
                    break;
                case EnumBarcodeStatus.已投递:
                case EnumBarcodeStatus.已退回:
                    return 0;
                case EnumBarcodeStatus.申请退回:
                    barCode.Status = EnumBarcodeStatus.已投递;
                    barCode.SubStatus = EnumBarcodeSubStatus.退回;
                    break;
            }
            //添加条码日志记录
            var preBarcodeLog = await _barcodeLogRepository.GetQueryable().Where(a => a.BarcodeNumber == barCodeNo)
                .OrderByDescending(a => a.Id).FirstOrDefaultAsync();

            var barcodeLog = new BarcodeLog
            {
                BarcodeNumber = barCodeNo,
                BarcodeStatus = EnumBarcodeStatus.已投递,
                DepartmentId = preBarcodeLog?.DepartmentId ?? place.DepartmentId,
                OperationTime = DateTime.Now,
                OperatorId = null
            };
            if (barCode.SubStatus == EnumBarcodeSubStatus.退回)
            {
                barcodeLog.Remark = "已投递申请退回的文件";
            }
            await _barcodeLogRepository.AddAsync(barcodeLog);

            //更新箱格信息
            var box = await _boxRepository.GetByIdAsync(boxId);
            box.FileCount += 1;
            box.HasUrgent = isJiaJi;

            return 1;
        }

        /// <summary>
        /// 用户取件
        /// </summary>
        /// <param name="boxId">箱子ID</param>
        /// <param name="cardValue">卡号</param>
        /// <param name="placeId">场所ID</param>
        /// <returns></returns>
        public async Task<int> UserGetLetter(int boxId, string cardValue, int placeId)
        {
            //箱子的流转对象签收人是否包含此人
            //不包含返回0
            //包含，依次签收箱内文件
            var user = await _cardRepository.GetQueryable().Where(a => a.Value == cardValue).Select(a => a.User).FirstOrDefaultAsync();
            if (user == null)
            {
                return 0;
            }

            var boxObjects = await _boxObjectRepository.GetQueryable().Where(a => a.BoxId == boxId).Select(a => a.ExchangeObject).ToListAsync();
            var objectSigners = await _objectSignerRepository.GetQueryable()
                .Where(a => boxObjects.Any(b => b.Id == a.ExchangeObjectId)).Select(a => a.User).ToListAsync();
            if (objectSigners.Any(a => a.Id == user.Id))
            {
                var barcodes = await _barcodeRepository.GetQueryable().Where(a =>
                     a.Status == EnumBarcodeStatus.已投递 && boxObjects.Any(b => b.Id == a.CurrentObjectId)).ToListAsync();
                foreach (var barcode in barcodes)
                {
                    barcode.Status = EnumBarcodeStatus.已签收;
                    if (barcode.UpdateTime != null)
                    {
                        var barcodeLog = new BarcodeLog
                        {
                            BarcodeNumber = barcode.BarcodeNo,
                            BarcodeStatus = EnumBarcodeStatus.已签收,
                            BarcodeSubStatus = barcode.SubStatus,
                            LastOperationTime = barcode.UpdateTime.Value,
                            DepartmentId = user.DepartmentId,
                            OperationTime = DateTime.Now,
                            OperatorId = user.Id
                        };
                        await _barcodeLogRepository.AddAsync(barcodeLog);
                    }

                    var firtExchangeObject = boxObjects.FirstOrDefault();
                    if (firtExchangeObject == null || firtExchangeObject.ObjectType != EnumObjectType.渠道) continue;
                    if (firtExchangeObject.Value != (int)EnumChannel.同城交换 &&
                        firtExchangeObject.Value != (int)EnumChannel.机要通信 &&
                        firtExchangeObject.Value != (int)EnumChannel.直送) continue;
                    //添加至待分拣列表
                    var sorting = new Sorting
                    {
                        Channel = (EnumChannel)firtExchangeObject.Value,
                        BarcodeNo = barcode.BarcodeNo
                    };
                    await _sortingRepository.AddAsync(sorting);
                }
                return 1;
            }
            return 0;
        }
    }
}
