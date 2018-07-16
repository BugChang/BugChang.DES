using System.Collections.Generic;
using System.Threading.Tasks;
using BugChang.DES.Application.Commons;
using BugChang.DES.Application.ExchangeObjects.Dtos;
using BugChang.DES.Core.Commons;

namespace BugChang.DES.Application.ExchangeObjects
{
    public interface IExchangeObjectAppService : ICurdAppService<ExchangeObjectEditDto, ExchangeObjectListDto>
    {
        IList<ObjectTypeListDto> GetObjectTypes();

        Task<IList<ExchangeObjectListDto>> GetAlListAsync();

        Task<ResultEntity> AssignObjectSigner(int objectId, List<int> userIds, int operatorId);

        Task<IList<int>> GetObjectSignerIds(int objectId);
    }
}
