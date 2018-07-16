using System.Collections.Generic;
using System.Threading.Tasks;
using BugChang.DES.Application.Commons;
using BugChang.DES.Application.Places.Dtos;
using BugChang.DES.Core.Commons;

namespace BugChang.DES.Application.Places
{
    public interface IPlaceAppService : ICurdAppService<PlaceEditDto, PlaceListDto>
    {
        Task<IList<PlaceListDto>> GetAllAsync();

        Task<IList<int>> GetPlaceWardenIds(int placeId);

        Task<ResultEntity> AssignWarden(int placeId, List<int> wardenIds, int operatorId);
    }
}
