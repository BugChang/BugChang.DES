using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BugChang.DES.Application.HardWares.Dtos;
using BugChang.DES.Core.Commons;
using BugChang.DES.Core.HardWares;
using BugChang.DES.EntityFrameWorkCore;

namespace BugChang.DES.Application.HardWares
{
    public class HardWareAppService : IHardWareAppService
    {
        private readonly IHardWareRepository _hardWareRepository;
        private readonly UnitOfWork _unitOfWork;

        public HardWareAppService(IHardWareRepository hardWareRepository, UnitOfWork unitOfWork)
        {
            _hardWareRepository = hardWareRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IList<HardWareDto>> GetSettings(string deviceCode)
        {
            var hardWares = await _hardWareRepository.GetSettings(deviceCode.Trim());
            return Mapper.Map<IList<HardWareDto>>(hardWares);
        }

        public async Task<HardWareDto> GetSettings(string deviceCode, EnumHardWareType hardWareType)
        {
            var hardWare = await _hardWareRepository.GetSettings(deviceCode.Trim(), hardWareType);
            return Mapper.Map<HardWareDto>(hardWare);
        }

        public async Task<ResultEntity> SaveHardWareSettings(List<HardWareSaveDto> hardWares, string deviceCode)
        {
            var result = new ResultEntity();
            var query = _hardWareRepository.GetQueryable().Where(a => a.DeviceCode == deviceCode);
            foreach (var hardWare in query)
            {
                await _hardWareRepository.DeleteByIdAsync(hardWare.Id);
            }

            foreach (var hardWareDto in hardWares)
            {
                var hardWare = Mapper.Map<HardWare>(hardWareDto);
                await _hardWareRepository.AddAsync(hardWare);
            }
            result.Success = true;
            await _unitOfWork.CommitAsync();
            return result;
        }
    }
}
