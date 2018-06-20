using System.Threading.Tasks;
using BugChang.DES.Core.Commons;

namespace BugChang.DES.Application.Commons
{
    public interface ICurdAppService<TEditDto, TListDto>
    {
        Task<ResultEntity> AddOrUpdateAsync(TEditDto editDto);

        Task<ResultEntity> DeleteByIdAsync(int id, int userId);

        Task<TEditDto> GetForEditByIdAsync(int id);

        Task<PageResultModel<TListDto>> GetPagingAysnc(PageSearchModel pageSearchDto);
    }
}
