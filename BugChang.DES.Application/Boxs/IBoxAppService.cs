using System.Collections.Generic;
using System.Threading.Tasks;
using BugChang.DES.Application.Boxs.Dtos;
using BugChang.DES.Application.Commons;
using BugChang.DES.Core.Commons;

namespace BugChang.DES.Application.Boxs
{
    public interface IBoxAppService : ICurdAppService<BoxEditDto, BoxListDto>
    {
        Task<IList<int>> GetBoxObjectIds(int boxId);

        Task<ResultEntity> AssignObject(int boxId, List<int> objectIds, int operatorId);

        Task<BoxEditDto> GetBoxByPlaceBn(string bn, int placeId);
    }
}
