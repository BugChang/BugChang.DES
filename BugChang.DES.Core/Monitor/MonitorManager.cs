using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Core.Authentication.Card;
using BugChang.DES.Core.Authorization.Users;
using BugChang.DES.Core.Exchanges.Barcodes;
using BugChang.DES.Core.Exchanges.Boxs;
using BugChang.DES.Core.Exchanges.ExchangeObjects;
using BugChang.DES.Core.Exchanges.Places;
using BugChang.DES.Core.Exchanges.Routes;
using BugChang.DES.Core.Letters;
using BugChang.DES.Core.Logs;
using Microsoft.EntityFrameworkCore;

namespace BugChang.DES.Core.Monitor
{
    public class MonitorManager
    {
        private readonly IPlaceRepository _placeRepository;
        private readonly IBarcodeRepository _barcodeRepository;
        private readonly IRouteRepository _routeRepository;
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
        public MonitorManager(IPlaceRepository placeRepository, IBarcodeRepository barcodeRepository, IRouteRepository routeRepository, IBoxObjectRepository boxObjectRepository, LogManager logManager, BarcodeManager barcodeManager, IBoxRepository boxRepository, ICardRepository cardRepository, IUserRepository userRepository, IPlaceWardenRepository placeWardenRepository, IExchangeObjectSignerRepository objectSignerRepository, ILetterRepository letterRepository, IBarcodeLogRepository barcodeLogRepository, IExchangeObjectRepository objectRepository)
        {
            _placeRepository = placeRepository;
            _barcodeRepository = barcodeRepository;
            _routeRepository = routeRepository;
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
        }

        public async Task<CheckBarcodeModel> CheckBarcodeType(string barcodeNo, int placeId)
        {

            var checkBarcodeModel = new CheckBarcodeModel();
            var barcode = await _barcodeRepository.GetByNoAsync(barcodeNo);
            var letter = await _letterRepository.GetLetter(barcodeNo) ?? new Letter
            {
                ReceiveDepartmentId = await _barcodeManager.GetReceiveDepartmentId(barcodeNo),
                SendDepartmentId = await _barcodeManager.GetSendDepartmentId(barcodeNo),
                //如果数据库中没有Letter记录，那么LetterType一定是收信
                LetterType = EnumLetterType.收信
            };

            switch (barcode.Status)
            {
                case EnumBarcodeStatus.已就绪:
                    break;
                case EnumBarcodeStatus.已投递:
                    checkBarcodeModel.Type = EnumCheckBarcodeType.条码已经投箱;
                    break;
                case EnumBarcodeStatus.已签收:
                    break;
                case EnumBarcodeStatus.已勘误:
                    break;
                case EnumBarcodeStatus.申请退回:
                    break;
                case EnumBarcodeStatus.已退回:
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
            return checkBarcodeModel;
        }


        private async Task<CheckBarcodeModel> CheckBarcodeTypeReady(Letter letter, int placeId)
        {
            var checkBarcodeModel = new CheckBarcodeModel();
            switch (letter.LetterType)
            {
                case EnumLetterType.收信:
                    var exchangeObject = await _objectRepository.GetQueryable().Where(a =>
                        a.ObjectType == EnumObjectType.机构 && a.Value == letter.ReceiveDepartmentId).FirstOrDefaultAsync();

                    var boxs = await _boxObjectRepository.GetQueryable()
                        .Where(a => a.Box.PlaceId == placeId && a.ExchangeObjectId == exchangeObject.Id)
                        .Select(a => a.Box).ToListAsync();
                    if (boxs.Count > 0)
                    {
                        checkBarcodeModel.Type = EnumCheckBarcodeType.唯一指定;
                        checkBarcodeModel.Record = boxs.Select(a => new CheckedBarcodeRecord { FileCount = 1, Message = "", No = a.Id }).ToList();
                    }
                    else
                    {
                    }
                    break;
                case EnumLetterType.发信:

                    break;
                case EnumLetterType.内交换:
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
                    CreateTime = DateTime.Now
                };
                barCode.BarcodeType = barCode.AnalysisBarcodeType(barCodeNo);
                await _barcodeRepository.AddAsync(barCode);
            }

            //添加条码记录
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
            await _barcodeLogRepository.AddAsync(barcodeLog);

            //更新箱格信息
            var box = await _boxRepository.GetByIdAsync(boxId);
            box.FileCount += 1;
            box.HasUrgent = isJiaJi;

            return 1;
        }
    }
}
