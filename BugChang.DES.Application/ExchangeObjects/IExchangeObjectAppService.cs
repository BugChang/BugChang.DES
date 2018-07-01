using System.Collections.Generic;
using BugChang.DES.Application.Commons;
using BugChang.DES.Application.ExchangeObjects.Dtos;

namespace BugChang.DES.Application.ExchangeObjects
{
    public interface IExchangeObjectAppService : ICurdAppService<ExchangeObjectEditDto, ExchangeObjectListDto>
    {
        IList<ObjectTypeListDto> GetObjectTypes();
    }
}
