using System.Threading.Tasks;
using AutoMapper;
using BugChang.DES.Application.BackUps.Dtos;
using BugChang.DES.Core.BackUps;
using BugChang.DES.Core.Commons;
using BugChang.DES.EntityFrameWorkCore;

namespace BugChang.DES.Application.BackUps
{
    public class BackUpAppService : IBackUpAppService
    {
        private readonly IBackUpRepository _backUpRepository;
        private readonly UnitOfWork _unitOfWork;

        public BackUpAppService(IBackUpRepository backUpRepository, UnitOfWork unitOfWork)
        {
            _backUpRepository = backUpRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResultEntity> AddOrUpdateAsync(BackUpEditDto editDto)
        {
            var result = new ResultEntity();
            var entity = Mapper.Map<DataBaseBackUp>(editDto);
            await _backUpRepository.AddAsync(entity);
            result.Success = true;
            await _unitOfWork.CommitAsync();
            return result;
        }

        public async Task<ResultEntity> DeleteByIdAsync(int id, int userId)
        {
            var result = new ResultEntity();
            var entity = await _backUpRepository.GetByIdAsync(id);
            entity.IsDeleted = true;
            await _unitOfWork.CommitAsync();
            result.Success = true;
            return result;
        }

        public Task<BackUpEditDto> GetForEditByIdAsync(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task<PageResultModel<BackUpListDto>> GetPagingAysnc(PageSearchCommonModel pageSearchDto)
        {
            throw new System.NotImplementedException();
        }
    }
}
