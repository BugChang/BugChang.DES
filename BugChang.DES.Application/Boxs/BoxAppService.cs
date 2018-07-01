using System;
using System.Threading.Tasks;
using AutoMapper;
using BugChang.DES.Application.Boxs.Dtos;
using BugChang.DES.Core.Commons;
using BugChang.DES.Core.Exchanges.Boxs;

namespace BugChang.DES.Application.Boxs
{
    public class BoxAppService : IBoxAppService
    {
        private readonly IBoxRepository _boxRepository;

        public BoxAppService(IBoxRepository boxRepository)
        {
            _boxRepository = boxRepository;
        }

        public Task<ResultEntity> AddOrUpdateAsync(BoxEditDto editDto)
        {
            throw new NotImplementedException();
        }

        public Task<ResultEntity> DeleteByIdAsync(int id, int userId)
        {
            throw new NotImplementedException();
        }

        public Task<BoxEditDto> GetForEditByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<PageResultModel<BoxListDto>> GetPagingAysnc(PageSearchModel pageSearchDto)
        {
            var boxs = await _boxRepository.GetPagingAysnc(pageSearchDto);
            return Mapper.Map<PageResultModel<BoxListDto>>(boxs);
        }
    }
}
