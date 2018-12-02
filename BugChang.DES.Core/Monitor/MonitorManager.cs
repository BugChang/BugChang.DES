using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Core.Authentication.Card;
using BugChang.DES.Core.Authorization.Users;
using BugChang.DES.Core.Commons;
using BugChang.DES.Core.Departments;
using BugChang.DES.Core.Exchanges.Barcodes;
using BugChang.DES.Core.Exchanges.Boxs;
using BugChang.DES.Core.Exchanges.Channel;
using BugChang.DES.Core.Exchanges.ExchangeObjects;
using BugChang.DES.Core.Exchanges.Places;
using BugChang.DES.Core.Letters;
using BugChang.DES.Core.SecretLevels;
using BugChang.DES.Core.Sortings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace BugChang.DES.Core.Monitor
{
    public class MonitorManager
    {
        private readonly IPlaceRepository _placeRepository;
        private readonly IBarcodeRepository _barcodeRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IBoxObjectRepository _boxObjectRepository;
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
        private readonly IOptions<CommonSettings> _commonSettings;
        private readonly ILogger<MonitorManager> _logger;
        public MonitorManager(IPlaceRepository placeRepository, IBarcodeRepository barcodeRepository, IBoxObjectRepository boxObjectRepository,
             BarcodeManager barcodeManager, IBoxRepository boxRepository, ICardRepository cardRepository, IUserRepository userRepository, IPlaceWardenRepository placeWardenRepository,
            IExchangeObjectSignerRepository objectSignerRepository, ILetterRepository letterRepository, IBarcodeLogRepository barcodeLogRepository, IExchangeObjectRepository objectRepository,
            IDepartmentRepository departmentRepository, ISortingRepository sortingRepository, IOptions<CommonSettings> commonSettings, ILogger<MonitorManager> logger)
        {
            _placeRepository = placeRepository;
            _barcodeRepository = barcodeRepository;
            _boxObjectRepository = boxObjectRepository;
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
            _commonSettings = commonSettings;
            _logger = logger;
        }

        public async Task<CheckBarcodeModel> CheckBarcodeType(string barcodeNo, int placeId)
        {

            _logger.LogWarning($"--------------开始检查条码：{barcodeNo}--------------");
            var checkBarcodeModel = new CheckBarcodeModel();
            if (barcodeNo.Length == 33 || barcodeNo.Length == 26 || barcodeNo.Contains("(01)000001500011"))
            {
                var barcode = await _barcodeRepository.GetByNoAsync(barcodeNo);

                var letter = await _letterRepository.GetLetter(barcodeNo);
                if (letter == null)
                {
                    letter = new Letter
                    {
                        BarcodeNo = barcodeNo,
                        ReceiveDepartmentId = await _barcodeManager.GetReceiveDepartmentId(barcodeNo),
                        SendDepartmentId = await _barcodeManager.GetSendDepartmentId(barcodeNo),
                        //如果数据库中没有Letter记录，那么LetterType一定是收信
                        LetterType = EnumLetterType.收信
                    };
                    letter.SecretLevel = letter.GetSecretLevel(barcodeNo);
                    letter.UrgencyLevel = letter.GetUrgencyLevel(barcodeNo);
                    letter.LetterNo = letter.GetLetterNo(barcodeNo);
                    letter.LetterType = GetLetterType(barcodeNo);
                }

                if (barcode != null)
                {
                    _logger.LogWarning($"当前流转状态：{barcode.Status.ToString()}");
                    switch (barcode.Status)
                    {
                        case EnumBarcodeStatus.已签收:
                            if (barcode.CurrentPlaceId == placeId && barcode.SubStatus == EnumBarcodeSubStatus.正常)
                            {
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
                            _logger.LogWarning($"重复投箱");
                            checkBarcodeModel.Type = EnumCheckBarcodeType.条码已经投箱;
                            break;
                        case EnumBarcodeStatus.申请退回:
                            _logger.LogWarning($"申请退回信件：对调收发单位信息");
                            var receiveDepartmentId = letter.ReceiveDepartmentId;
                            letter.ReceiveDepartmentId = letter.SendDepartmentId;
                            letter.SendDepartmentId = receiveDepartmentId;
                            if (letter.LetterType == EnumLetterType.发信)
                            {
                                letter.LetterType = EnumLetterType.收信;
                            }
                            else if (letter.LetterType == EnumLetterType.收信)
                            {
                                letter.LetterType = EnumLetterType.发信;
                            }
                            return await CheckBarcodeTypeCommon(letter, placeId);
                        case EnumBarcodeStatus.已退回:
                            _logger.LogWarning($"条码无效：已退回的文件");
                            checkBarcodeModel.Type = EnumCheckBarcodeType.无效;
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                else
                {
                    _logger.LogWarning($"不存在流转记录");
                    return await CheckBarcodeTypeCommon(letter, placeId);
                }
            }
            _logger.LogWarning($"--------------结束检查条码：{JsonConvert.SerializeObject(checkBarcodeModel)}--------------");
            return checkBarcodeModel;
        }


        private async Task<CheckBarcodeModel> CheckBarcodeTypeCommon(Letter letter, int placeId)
        {
            _logger.LogWarning($"进入通用逻辑");
            var checkBarcodeModel = new CheckBarcodeModel();

            if (letter.SendDepartmentId == 0)
            {
                _logger.LogWarning($"发件单位错误");
                return checkBarcodeModel;
            }


            _logger.LogWarning($"信件类型：{letter.LetterType.ToString()}");
            switch (letter.LetterType)
            {
                case EnumLetterType.收信:
                    {
                        var exchangeObject = await _objectRepository.GetQueryable().Where(a =>
                                a.ObjectType == EnumObjectType.机构 && a.Value == letter.ReceiveDepartmentId)
                            .FirstOrDefaultAsync();

                        if (exchangeObject == null)
                        {
                            _logger.LogWarning($"不存在的收件流转对象");
                            checkBarcodeModel.Type = EnumCheckBarcodeType.无效;
                            return checkBarcodeModel;
                        }
                        var objectId = exchangeObject.Id;
                        var boxs = await _boxObjectRepository.GetQueryable()
                            .Where(a => a.Box.PlaceId == placeId && a.ExchangeObjectId == objectId && a.Box.Enabled && a.ExchangeObject.ObjectType != EnumObjectType.渠道)
                            .Select(a => a.Box).ToListAsync();

                        if (boxs.Count > 0)
                        {
                            _logger.LogWarning($"结束：当前场所存在收件箱格");
                            checkBarcodeModel.Type = EnumCheckBarcodeType.唯一指定;
                            checkBarcodeModel.Record =
                                boxs.Select(a => new CheckedBarcodeRecord { FileCount = 1, Message = "", No = a.Id })
                                    .ToList();
                        }
                        else
                        {
                            _logger.LogWarning($"递归上级流转对象在本场所有箱格");
                            var parentObject = exchangeObject;
                            while (parentObject.ParentId != null)
                            {
                                var o = parentObject;
                                var tempParentObject = await _objectRepository.GetQueryable().Where(a => a.Id == o.ParentId.Value)
                                     .FirstOrDefaultAsync();
                                boxs = await _boxObjectRepository.GetQueryable()
                                    .Where(a => a.Box.PlaceId == placeId && a.ExchangeObjectId == tempParentObject.Id && a.Box.Enabled)
                                    .Select(a => a.Box).ToListAsync();
                                if (boxs.Count > 0)
                                {
                                    _logger.LogWarning($"结束：存在上级流转对象箱格");
                                    checkBarcodeModel.Type = EnumCheckBarcodeType.唯一指定;
                                    checkBarcodeModel.Record =
                                        boxs.Select(a => new CheckedBarcodeRecord { FileCount = 1, Message = "", No = a.Id })
                                            .ToList();
                                    return checkBarcodeModel;
                                }

                                parentObject = tempParentObject;
                            }

                            if (exchangeObject.IsVirtual)
                            {
                                _logger.LogWarning($"收件流转对象为虚拟对象");
                                var childBoxs = await _boxObjectRepository.GetQueryable()
                                      .Where(a => a.Box.PlaceId == placeId && a.ExchangeObject.ParentId == exchangeObject.Id && a.Box.Enabled)
                                      .Select(a => a.Box).ToListAsync();
                                if (childBoxs.Count > 0)
                                {

                                    checkBarcodeModel.Type = EnumCheckBarcodeType.唯一指定;
                                    checkBarcodeModel.Record =
                                        childBoxs.Select(a => new CheckedBarcodeRecord { FileCount = 1, Message = "", No = a.Id })
                                            .ToList();
                                    _logger.LogWarning($"结束：存在收件流转对象下级的箱格");
                                    return checkBarcodeModel;
                                }

                                boxs = await _boxObjectRepository.GetQueryable()
                                    .Where(a => a.Box.PlaceId == placeId && a.Box.Enabled && a.ExchangeObject.ObjectType != EnumObjectType.渠道)
                                    .Select(a => a.Box).ToListAsync();
                                checkBarcodeModel.Type = EnumCheckBarcodeType.唯一指定;
                                checkBarcodeModel.Record =
                                    boxs.Select(a => new CheckedBarcodeRecord { FileCount = 1, Message = "", No = a.Id })
                                        .ToList();
                                _logger.LogWarning($"结束：除渠道箱外全部返回");
                                return checkBarcodeModel;
                            }

                        }

                        break;
                    }
                case EnumLetterType.发信:
                    {
                        var receiveDepartment = await _departmentRepository.GetByIdAsync(letter.ReceiveDepartmentId);
                        var receiveChannel = receiveDepartment.ReceiveChannel;
                        if (letter.SecretLevel == EnumSecretLevel.绝密)
                        {
                            receiveChannel = EnumChannel.机要通信;
                        }
                        var channelExchangeObjects = await _objectRepository.GetQueryable().Where(a =>
                                a.ObjectType == EnumObjectType.渠道 && a.Value == (int)receiveChannel)
                            .ToListAsync();
                        if (channelExchangeObjects.Count > 0)
                        {
                            var boxs = await _boxObjectRepository.GetQueryable()
                                .Where(a => a.Box.PlaceId == placeId && channelExchangeObjects.Exists(b => b.Id == a.ExchangeObjectId) && a.Box.Enabled)
                                .Select(a => a.Box).ToListAsync();
                            if (boxs.Count > 0)
                            {
                                checkBarcodeModel.Type = EnumCheckBarcodeType.唯一指定;
                                checkBarcodeModel.Record = boxs.Select(a =>
                                    new CheckedBarcodeRecord { FileCount = 1, Message = "", No = a.Id }).ToList();
                                _logger.LogWarning($"结束：存在对外的渠道箱");
                                return checkBarcodeModel;
                            }

                            var currentSend = false;
                            var place = await _placeRepository.GetByIdAsync(placeId);
                            var sendDepartment = await _departmentRepository.GetByIdAsync(letter.SendDepartmentId);
                            while (sendDepartment.ParentId != null)
                            {
                                if (place.DepartmentId == sendDepartment.Id)
                                {
                                    currentSend = true;
                                    break;
                                }

                                sendDepartment = await _departmentRepository.GetByIdAsync(sendDepartment.ParentId.Value);
                            }

                            if (currentSend)
                            {
                                var insideExchangeObjects = await _objectRepository.GetQueryable().Where(a =>
                                        a.ObjectType == EnumObjectType.渠道 && a.Value == (int)EnumChannel.内部)
                                    .ToListAsync();
                                List<ExchangeObject> matchExchangeObjects;
                                if (receiveChannel == EnumChannel.同城交换 && letter.SecretLevel != EnumSecretLevel.绝密)
                                {
                                    matchExchangeObjects = insideExchangeObjects.Where(a => a.RestrictionCode.Contains("同城交换")).ToList();
                                }
                                else
                                {
                                    matchExchangeObjects = insideExchangeObjects.Where(a => a.RestrictionCode.Contains("机要通信")).ToList();
                                }

                                if (matchExchangeObjects.Count > 0)
                                {
                                    _logger.LogWarning($"结束：内部限制码渠道箱匹配成功");
                                    boxs = await _boxObjectRepository.GetQueryable()
                                        .Where(a => a.Box.PlaceId == placeId && matchExchangeObjects.Any(b => b.Id == a.ExchangeObjectId) && a.Box.Enabled)
                                        .Select(a => a.Box).ToListAsync();
                                    if (boxs.Count == 0)
                                    {
                                        _logger.LogWarning($"结束：内部渠道箱匹配成功");
                                        boxs = await _boxObjectRepository.GetQueryable()
                                            .Where(a => a.Box.PlaceId == placeId && insideExchangeObjects.Any(b => b.Id == a.ExchangeObjectId) && a.Box.Enabled)
                                            .Select(a => a.Box).ToListAsync();
                                    }
                                }
                                if (boxs.Count > 0)
                                {
                                    checkBarcodeModel.Type = EnumCheckBarcodeType.唯一指定;
                                    checkBarcodeModel.Record = boxs.Select(a =>
                                        new CheckedBarcodeRecord { FileCount = 1, Message = "", No = a.Id }).ToList();
                                }
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
                        var exchangeObject = await _objectRepository.GetQueryable().Where(a =>
                                a.ObjectType == EnumObjectType.机构 && a.Value == letter.ReceiveDepartmentId)
                            .FirstOrDefaultAsync();

                        if (exchangeObject == null)
                        {
                            _logger.LogWarning($"不存在的收件流转对象");
                            checkBarcodeModel.Type = EnumCheckBarcodeType.无效;
                            return checkBarcodeModel;
                        }
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
                            _logger.LogWarning($"结束：当前场所下存在收件流转对象箱格");
                            return checkBarcodeModel;
                        }

                        var parentObject = exchangeObject;
                        while (parentObject.ParentId != null)
                        {
                            var o = parentObject;
                            var tempParentObject = await _objectRepository.GetQueryable().Where(a => a.Id == o.ParentId.Value)
                                .FirstOrDefaultAsync();
                            boxs = await _boxObjectRepository.GetQueryable()
                                .Where(a => a.Box.PlaceId == placeId && a.ExchangeObjectId == tempParentObject.Id && a.Box.Enabled)
                                .Select(a => a.Box).ToListAsync();
                            if (boxs.Count > 0)
                            {
                                checkBarcodeModel.Type = EnumCheckBarcodeType.唯一指定;
                                checkBarcodeModel.Record =
                                    boxs.Select(a => new CheckedBarcodeRecord { FileCount = 1, Message = "", No = a.Id })
                                        .ToList();
                                _logger.LogWarning($"结束：存在流转对象上级对象箱格");
                                return checkBarcodeModel;
                            }

                            parentObject = tempParentObject;
                        }

                        if (exchangeObject.IsVirtual)
                        {
                            _logger.LogWarning($"流转对象为虚拟流转对象");
                            var childBoxs = await _boxObjectRepository.GetQueryable()
                                .Where(a => a.Box.PlaceId == placeId && a.ExchangeObject.ParentId == exchangeObject.Id && a.Box.Enabled)
                                .Select(a => a.Box).ToListAsync();
                            if (childBoxs.Count > 0)
                            {
                                _logger.LogWarning($"结束：存在流转对象子对象箱格");
                                checkBarcodeModel.Type = EnumCheckBarcodeType.唯一指定;
                                checkBarcodeModel.Record =
                                    childBoxs.Select(a => new CheckedBarcodeRecord { FileCount = 1, Message = "", No = a.Id })
                                        .ToList();
                                return checkBarcodeModel;
                            }
                        }

                        var currentSend = false;
                        //判断是否是该场所下的单位发件
                        var place = await _placeRepository.GetByIdAsync(placeId);
                        var sendDepartment = await _departmentRepository.GetByIdAsync(letter.SendDepartmentId);
                        while (sendDepartment.ParentId != null)
                        {
                            if (place.DepartmentId == sendDepartment.Id)
                            {
                                currentSend = true;
                                break;
                            }
                            sendDepartment = await _departmentRepository.GetByIdAsync(sendDepartment.ParentId.Value);
                        }

                        if (currentSend)
                        {
                            _logger.LogWarning($"当前场所下单位发件");
                            //当前场所下单位发件
                            var exchangeObjects = await _objectRepository.GetQueryable().Where(a =>
                                    a.ObjectType == EnumObjectType.渠道 && a.Value == (int)EnumChannel.内部)
                                .ToListAsync();
                            var receiveCode = letter.GetReceiveCode(letter.BarcodeNo);
                            var matchExchangeObjects = exchangeObjects.Where(a => receiveCode.Contains(a.RestrictionCode)).ToList();
                            if (matchExchangeObjects.Count > 0)
                            {
                                var c = JsonConvert.SerializeObject(matchExchangeObjects);

                                if (matchExchangeObjects.Count > 1)
                                {
                                    //匹配数量超过一个，排除限制码为空的对象
                                    matchExchangeObjects = exchangeObjects.Where(a => receiveCode.Contains(a.RestrictionCode) && !string.IsNullOrWhiteSpace(a.RestrictionCode)).ToList();
                                }
                                boxs = await _boxObjectRepository.GetQueryable()
                                    .Where(a => a.Box.PlaceId == placeId && matchExchangeObjects.Any(b => b.Id == a.ExchangeObjectId) && a.Box.Enabled)
                                    .Select(a => a.Box).ToListAsync();
                                _logger.LogWarning($"结束：内部限制码渠道箱匹配成功");
                            }
                            else
                            {
                                boxs = await _boxObjectRepository.GetQueryable()
                                    .Where(a => a.Box.PlaceId == placeId && exchangeObjects.Any(b => b.Id == a.ExchangeObjectId) && a.Box.Enabled)
                                    .Select(a => a.Box).ToListAsync();
                                _logger.LogWarning($"结束：内部渠道箱匹配成功");
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
                    break;
            }
            return checkBarcodeModel;
        }

        /// <summary>
        /// 虚拟流转对象逻辑
        /// </summary>
        /// <param name="placeId"></param>
        /// <param name="objectId"></param>
        /// <returns></returns>
        private async Task<CheckBarcodeModel> CheckVirtualObject(int placeId, int objectId)
        {
            _logger.LogWarning($"虚拟流转对象");
            var checkBarcodeModel = new CheckBarcodeModel();
            var boxs = await _boxObjectRepository.GetQueryable()
                 .Where(a => a.Box.PlaceId == placeId && a.ExchangeObject.ParentId == objectId && a.Box.Enabled && a.ExchangeObject.ObjectType != EnumObjectType.渠道)
                 .Select(a => a.Box).ToListAsync();
            if (boxs.Count > 0)
            {
                _logger.LogWarning($"虚拟流转对象存在下级流转对象并在该交换场所存在箱格");
                checkBarcodeModel.Type = EnumCheckBarcodeType.唯一指定;
                checkBarcodeModel.Record =
                    boxs.Select(a => new CheckedBarcodeRecord { FileCount = 1, Message = "", No = a.Id })
                        .ToList();
            }
            else
            {
                _logger.LogWarning($"除渠道箱外全部返回");
                boxs = await _boxObjectRepository.GetQueryable()
                    .Where(a => a.Box.PlaceId == placeId && a.Box.Enabled && a.ExchangeObject.ObjectType != EnumObjectType.渠道)
                    .Select(a => a.Box).ToListAsync();
                checkBarcodeModel.Type = EnumCheckBarcodeType.唯一指定;
                checkBarcodeModel.Record =
                    boxs.Select(a => new CheckedBarcodeRecord { FileCount = 1, Message = "", No = a.Id })
                        .ToList();
            }
            return checkBarcodeModel;
        }


        /// <summary>
        ///  //查找上级流转对象逻辑
        /// </summary>
        /// <param name="exchangeObject"></param>
        /// <param name="placeId"></param>
        /// <returns></returns>
        private async Task<CheckBarcodeModel> CheckParent(ExchangeObject exchangeObject, int placeId)
        {
            _logger.LogWarning($"循环判断上级流转对象在这有没有箱子");
            var checkBarcodeModel = new CheckBarcodeModel();
            while (exchangeObject.ParentId != null)
            {
                var o = exchangeObject;
                exchangeObject = await _objectRepository.GetQueryable().Where(a => a.Id == o.ParentId.Value)
                    .FirstOrDefaultAsync();
                var exchangeObject1 = exchangeObject;
                var boxs = await _boxObjectRepository.GetQueryable()
                     .Where(a => a.Box.PlaceId == placeId && a.ExchangeObjectId == exchangeObject1.Id && a.Box.Enabled)
                     .Select(a => a.Box).ToListAsync();
                if (boxs.Count > 0)
                {
                    checkBarcodeModel.Type = EnumCheckBarcodeType.唯一指定;
                    checkBarcodeModel.Record = boxs.Select(a =>
                        new CheckedBarcodeRecord { FileCount = 1, Message = "", No = a.Id }).ToList();
                    return checkBarcodeModel;
                }
                if (exchangeObject.IsVirtual)
                {
                    return await CheckVirtualObject(placeId, exchangeObject.Id);
                }
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
            //箱头数量改用动态计算
            var objectIds = await _boxObjectRepository.GetQueryable().Where(a => a.BoxId == boxId)
                .Select(a => a.ExchangeObjectId).ToListAsync();
            box.FileCount = await _barcodeRepository.GetQueryable()
                .Where(a => objectIds.Contains(a.CurrentObjectId) && a.Status == EnumBarcodeStatus.已投递).CountAsync();
            return box;
        }

        public async Task<CheckCardTypeModel> CheckCardType(int placeId, int boxId, string cardValue)
        {
            _logger.LogWarning($"--------------开始检查证卡：{cardValue}--------------");
            var card = await _cardRepository.GetQueryable().Where(a => a.Value == cardValue).FirstOrDefaultAsync();
            if (card == null)
            {
                return new CheckCardTypeModel();
            }
            _logger.LogWarning($"证卡号：{card.Number},证卡值：{card.Value}");
            var user = await _userRepository.GetQueryable().Include(a => a.Department).FirstOrDefaultAsync(a => a.Id == card.UserId);
            _logger.LogWarning($"所属用户：{user.DisplayName}");
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
                _logger.LogWarning($"交换员身份：用户是该流转对象签收人");
            }
            else
            {
                var placeWarden = await _placeWardenRepository.GetQueryable().Where(a => a.PlaceId == placeId && a.UserId == user.Id).FirstOrDefaultAsync();
                if (placeWarden != null)
                {
                    //当前刷卡人为本场所管理员
                    _logger.LogWarning($"管理员身份：当前刷卡人为本场所管理员");
                    checkCardModel.Type = 21;
                }
            }
            _logger.LogWarning($"--------------结束检查证卡--------------");
            return checkCardModel;
        }

        public async Task<int> SaveLetter(int placeId, string barCodeNo, int boxId, int fileCount, bool isJiaJi)
        {
            _logger.LogWarning($"--------------开始保存条码：{barCodeNo}--------------");
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
                letter.LetterNo = letter.GetLetterNo(barCodeNo);
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
                    CurrentObjectId = boxObject.ExchangeObjectId,
                    CurrentPlaceId = placeId
                };
                barCode.BarcodeType = barCode.AnalysisBarcodeType(barCodeNo);
                await _barcodeRepository.AddAsync(barCode);
            }

            barCode.CurrentObjectId = boxObject.ExchangeObjectId;
            barCode.CurrentPlaceId = placeId;
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
                    break;
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
                OperatorId = null,
                CurrentObjectId = boxObject.ExchangeObjectId,
                CurrentPlaceId = placeId
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
            _logger.LogWarning($"--------------结束保存条码--------------");
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
            _logger.LogWarning($"--------------用户取件开始--------------");
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
                _logger.LogWarning($"barcodes.Count:{barcodes.Count}");
                foreach (var barcode in barcodes)
                {
                    barcode.Status = EnumBarcodeStatus.已签收;
                    barcode.CurrentPlaceId = placeId;
                    barcode.CurrentObjectId = boxObjects[0].Id;
                    var barcodeLog = new BarcodeLog
                    {
                        BarcodeNumber = barcode.BarcodeNo,
                        BarcodeStatus = EnumBarcodeStatus.已签收,
                        BarcodeSubStatus = barcode.SubStatus,
                        LastOperationTime = barcode.UpdateTime ?? DateTime.Now,
                        DepartmentId = user.DepartmentId,
                        OperationTime = DateTime.Now,
                        OperatorId = user.Id,
                        CurrentPlaceId = placeId,
                        CurrentObjectId = boxObjects[0].Id
                    };

                    await _barcodeLogRepository.AddAsync(barcodeLog);

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

                var box = await _boxRepository.GetByIdAsync(boxId);
                box.HasUrgent = false;
                box.FileCount = 0;
                _logger.LogWarning($"--------------用户取件结束：1--------------");
                return 1;
            }
            _logger.LogWarning($"--------------用户取件结束：0--------------");
            return 0;
        }

        public EnumLetterType GetLetterType(string barcodeNo)
        {
            var useDepartmentCode = _commonSettings.Value.UseDepartmentCode;
            if (barcodeNo.Length == 26)
            {
                return barcodeNo.Substring(1, 3) == useDepartmentCode ? EnumLetterType.发信 : EnumLetterType.收信;
            }

            if (barcodeNo.Length == 33)
            {
                if (barcodeNo.Substring(0, 3) == useDepartmentCode && barcodeNo.Substring(22, 3) == useDepartmentCode)
                {
                    return EnumLetterType.内交换;
                }
                return barcodeNo.Substring(0, 3) == useDepartmentCode ? EnumLetterType.发信 : EnumLetterType.收信;
            }

            return EnumLetterType.收信;
        }
    }
}
