using System.Collections.Generic;
using System.Threading.Tasks;
using BugChang.DES.Application.HardWares.Dtos;
using BugChang.DES.Core.Commons;

namespace BugChang.DES.Application.HardWares
{
    public interface IHardWareAppService
    {
        Task<IList<HardWareDto>> GetSettings(string deviceCode);

        Task<ResultEntity> SaveHardWareSettings(List<HardWareSaveDto> hardWares,string deviceCode);
    }
}
