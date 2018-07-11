using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BugChang.DES.Application.ExchangeObjects.Dtos;
using BugChang.DES.Core.Commons;
using BugChang.DES.Core.Exchanges.ExchangeObjects;
using BugChang.DES.EntityFrameWorkCore;

namespace BugChang.DES.Application.ExchangeObjects
{
    public class ExchangeObjectAppService : IExchangeObjectAppService
    {
        private readonly IExchangeObjectRepository _exchangeObjectRepository;
        private readonly ExchangeObjectManager _exchangeObjectManager;
        private readonly UnitOfWork _unitOfWork;

        public ExchangeObjectAppService(IExchangeObjectRepository exchangeObjectRepository, ExchangeObjectManager exchangeObjectManager, UnitOfWork unitOfWork)
        {
            _exchangeObjectRepository = exchangeObjectRepository;
            _exchangeObjectManager = exchangeObjectManager;
            _unitOfWork = unitOfWork;
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

        public async Task<PageResultModel<ExchangeObjectListDto>> GetPagingAysnc(PageSearchModel pageSearchDto)
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
            var exchangeObjects= await _exchangeObjectRepository.GetAllAsync();
            return Mapper.Map<IList<ExchangeObjectListDto>>(exchangeObjects);
        }
    }
}
