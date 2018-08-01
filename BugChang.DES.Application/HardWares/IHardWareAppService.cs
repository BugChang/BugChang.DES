using System.Collections.Generic;
using System.Threading.Tasks;
using BugChang.DES.Application.HardWares.Dtos;
using BugChang.DES.Core.Commons;
using BugChang.DES.Core.HardWares;

namespace BugChang.DES.Application.HardWares
{
    public interface IHardWareAppService
    {
        Task<IList<HardWareDto>> GetSettings(string deviceCode);
        Task<HardWareDto> GetSettings(string deviceCode, EnumHardWareType hardWareType);

        Task<ResultEntity> SaveHardWareSettings(List<HardWareSaveDto> hardWares, string deviceCode);
    }
}
