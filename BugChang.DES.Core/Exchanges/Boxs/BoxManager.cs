using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Core.Commons;
using Microsoft.EntityFrameworkCore;

namespace BugChang.DES.Core.Exchanges.Boxs
{
    public class BoxManager
    {
        private readonly IBoxRepository _boxRepository;

        public BoxManager(IBoxRepository boxRepository)
        {
            _boxRepository = boxRepository;
        }

        public async Task<ResultEntity> AddOrUpdateAsync(Box box)
        {
            var result = new ResultEntity();
            var exist = await _boxRepository.GetQueryable().Where(a => a.DeviceCode == box.DeviceCode && a.PlaceId == box.PlaceId && a.Id != box.Id).CountAsync() > 0;
            if (exist)
            {
                result.Message = "同一交换场所下设备编码不允许重复";
            }
            else
            {
                if (box.Id > 0)
                {
                    _boxRepository.Update(box);
                }
                else
                {
                    await _boxRepository.AddAsync(box);
                }

                result.Success = true;
            }

            return result;
        }

        public async Task<ResultEntity> DeleteAsync(int boxId)
        {
            var result = new ResultEntity();
            var box = await _boxRepository.GetByIdAsync(boxId);
            box.IsDeleted = true;
            result.Success = true;
            return result;
        }
    }
}
