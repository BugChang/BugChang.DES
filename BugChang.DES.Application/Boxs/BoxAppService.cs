using System;
using System.Threading.Tasks;
using AutoMapper;
using BugChang.DES.Application.Boxs.Dtos;
using BugChang.DES.Core.Commons;
using BugChang.DES.Core.Exchanges.Boxs;
using BugChang.DES.EntityFrameWorkCore;

namespace BugChang.DES.Application.Boxs
{
    public class BoxAppService : IBoxAppService
    {
        private readonly IBoxRepository _boxRepository;
        private readonly BoxManager _boxManager;
        private readonly UnitOfWork _unitOfWork;

        public BoxAppService(IBoxRepository boxRepository, BoxManager boxManager, UnitOfWork unitOfWork)
        {
            _boxRepository = boxRepository;
            _boxManager = boxManager;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResultEntity> AddOrUpdateAsync(BoxEditDto editDto)
        {
            var result = await _boxManager.AddOrUpdateAsync(Mapper.Map<Box>(editDto));
            if (result.Success)
            {
                await _unitOfWork.CommitAsync();
            }
            return result;
        }

        public async Task<ResultEntity> DeleteByIdAsync(int id, int userId)
        {
            var result = await _boxManager.DeleteAsync(id);
            if (result.Success)
            {
                await _unitOfWork.CommitAsync();
            }
            return result;
        }

        public async Task<BoxEditDto> GetForEditByIdAsync(int id)
        {
            var box = await _boxRepository.GetByIdAsync(id);
            return Mapper.Map<BoxEditDto>(box);
        }

        public async Task<PageResultModel<BoxListDto>> GetPagingAysnc(PageSearchModel pageSearchDto)
        {
            var pageResult = await _boxRepository.GetPagingAysnc(pageSearchDto);
            return Mapper.Map<PageResultModel<BoxListDto>>(pageResult);
        }
    }
}
