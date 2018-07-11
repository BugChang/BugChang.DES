using System.Collections.Generic;
using System.Threading.Tasks;
using BugChang.DES.Application.Commons;
using BugChang.DES.Application.ExchangeObjects.Dtos;

namespace BugChang.DES.Application.ExchangeObjects
{
    public interface IExchangeObjectAppService : ICurdAppService<ExchangeObjectEditDto, ExchangeObjectListDto>
    {
        IList<ObjectTypeListDto> GetObjectTypes();

        Task<IList<ExchangeObjectListDto>> GetAlListAsync();
    }
}
