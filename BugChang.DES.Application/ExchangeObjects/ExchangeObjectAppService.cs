using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BugChang.DES.Application.ExchangeObjects.Dtos;
using BugChang.DES.Core.Commons;
using BugChang.DES.Core.Exchanges.ExchangeObjects;
using BugChang.DES.Core.Logs;
using BugChang.DES.EntityFrameWorkCore;
using Newtonsoft.Json;

namespace BugChang.DES.Application.ExchangeObjects
{
    public class ExchangeObjectAppService : IExchangeObjectAppService
    {
        private readonly IExchangeObjectRepository _exchangeObjectRepository;
        private readonly ExchangeObjectManager _exchangeObjectManager;
        private readonly LogManager _logManager;
        private readonly UnitOfWork _unitOfWork;

        public ExchangeObjectAppService(IExchangeObjectRepository exchangeObjectRepository, ExchangeObjectManager exchangeObjectManager, UnitOfWork unitOfWork, LogManager logManager)
        {
            _exchangeObjectRepository = exchangeObjectRepository;
            _exchangeObjectManager = exchangeObjectManager;
            _unitOfWork = unitOfWork;
            _logManager = logManager;
        }

        public async Task<ResultEntity> AddOrUpdateAsync(ExchangeObjectEditDto editDto)
        {
            var exchangeObject = Mapper.Map<ExchangeObject>(editDto);
            var result = await _exchangeObjectManager.AddOrUpdateAsync(exchangeObject);
            if (result.Success)
            {
                await _unitOfWork.CommitAsync();
            }

            return result;
        }

        public async Task<ResultEntity> DeleteByIdAsync(int id, int userId)
        {
            var result = await _exchangeObjectManager.DeleteByIdAsync(id);
            if (result.Success)
            {
                await _unitOfWork.CommitAsync();
            }

            return result;
        }

        public async Task<ExchangeObjectEditDto> GetForEditByIdAsync(int id)
        {
            var exchangeObject = await _exchangeObjectRepository.GetByIdAsync(id);
            return Mapper.Map<ExchangeObjectEditDto>(exchangeObject);
        }

        public async Task<PageResultModel<ExchangeObjectListDto>> GetPagingAysnc(PageSearchCommonModel pageSearchDto)
        {
            var exchangeObjects = await _exchangeObjectRepository.GetPagingAysnc(pageSearchDto);
            return Mapper.Map<PageResultModel<ExchangeObjectListDto>>(exchangeObjects);
        }

        public IList<ObjectTypeListDto> GetObjectTypes()
        {
            var objectTypeList = new List<ObjectTypeListDto>();
            foreach (var item in Enum.GetValues(typeof(EnumObjectType)))
            {
                var objectType = new ObjectTypeListDto
                {
                    Id = (int)item,
                    Name = item.ToString()
                };
                objectTypeList.Add(objectType);
            }

            return objectTypeList;
        }

        public async Task<IList<ExchangeObjectListDto>> GetAlListAsync()
        {
            var exchangeObjects = await _exchangeObjectRepository.GetAllAsync();
            return Mapper.Map<IList<ExchangeObjectListDto>>(exchangeObjects);
        }

        public async Task<ResultEntity> AssignObjectSigner(int objectId, List<int> userIds, int operatorId)
        {
            var result = await _exchangeObjectManager.AssignObjectSigner(objectId, userIds);
            if (result.Success)
            {
                await _unitOfWork.CommitAsync();
                await _logManager.LogInfomationAsync(EnumLogType.Audit, LogTitleConstString.ObjectSignerAssign, result.Message,
                    JsonConvert.SerializeObject(result.Data), operatorId);
            }
            return result;
        }

        public async Task<IList<int>> GetObjectSignerIds(int objectId)
        {
            return await _exchangeObjectManager.GetObjectSignerIds(objectId);
        }

        public async Task<IList<ExchangeObjectListDto>> GetObjects(int signerId, int placeId)
        {
            var objects = await _exchangeObjectManager.GetObjects(signerId, placeId);
            return Mapper.Map<IList<ExchangeObjectListDto>>(objects);
        }
    }
}
