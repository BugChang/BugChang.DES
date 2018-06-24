using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BugChang.DES.Application.Places.Dtos;
using BugChang.DES.Core.Commons;
using BugChang.DES.Core.Exchanges.Places;
using BugChang.DES.Core.Logs;
using BugChang.DES.EntityFrameWorkCore;
using Newtonsoft.Json;

namespace BugChang.DES.Application.Places
{
    public class PlaceAppService : IPlaceAppService
    {
        private readonly IPlaceRepository _placeRepository;
        private readonly PlaceManager _placeManager;
        private readonly LogManager _logManager;
        private readonly UnitOfWork _unitOfWork;

        public PlaceAppService(PlaceManager placeManager, UnitOfWork unitOfWork, LogManager logManager, IPlaceRepository placeRepository)
        {
            _placeManager = placeManager;
            _unitOfWork = unitOfWork;
            _logManager = logManager;
            _placeRepository = placeRepository;
        }

        public async Task<ResultEntity> AddOrUpdateAsync(PlaceEditDto place)
        {
            var model = Mapper.Map<Place>(place);
            var result = await _placeManager.AddOrUpdateAsync(model);
            if (!result.Success) return result;
            await _unitOfWork.CommitAsync();
            if (place.Id > 0)
                await _logManager.LogInfomationAsync(EnumLogType.Audit, LogTitleConstString.PlaceEdit, $"{place.Name}", JsonConvert.SerializeObject(place), place.UpdateBy);
            else
                await _logManager.LogInfomationAsync(EnumLogType.Audit, LogTitleConstString.PlaceAdd, $"{place.Name}", JsonConvert.SerializeObject(place), place.CreateBy);
            return result;
        }

        public Task<ResultEntity> DeleteByIdAsync(int id, int userId)
        {
            throw new NotImplementedException();
        }

        public async Task<PlaceEditDto> GetForEditByIdAsync(int id)
        {
            return Mapper.Map<PlaceEditDto>(await _placeRepository.GetByIdAsync(id));
        }

        public async Task<PageResultModel<PlaceListDto>> GetPagingAysnc(PageSearchModel pageSearchDto)
        {
            var pageResult = await _placeRepository.GetPagingAysnc(pageSearchDto);
            return Mapper.Map<PageResultModel<PlaceListDto>>(pageResult);
        }

        public async Task<IList<PlaceListDto>> GetAllAsync()
        {
            var places = await _placeRepository.GetAllAsync();
            return Mapper.Map<IList<PlaceListDto>>(places);
        }
    }
}
