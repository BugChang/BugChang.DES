using System.Collections.Generic;
using System.Threading.Tasks;
using BugChang.DES.Application.Commons;
using BugChang.DES.Application.Places.Dtos;

namespace BugChang.DES.Application.Places
{
    public interface IPlaceAppService : ICurdAppService<PlaceEditDto, PlaceListDto>
    {
        Task<IList<PlaceListDto>> GetAllAsync();
    }
}
