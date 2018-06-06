using System.Threading.Tasks;
using BugChang.DES.Core.Common;

namespace BugChang.DES.Application.Commons
{
    public interface ICurdAppService<TEntityDto> where TEntityDto : class
    {
        Task<ResultEntity> AddOrUpdateAsync(TEntityDto dto);

        Task<ResultEntity> DeleteAsync(int id);

        Task<TEntityDto> GetAsync(int id);
    }
}
