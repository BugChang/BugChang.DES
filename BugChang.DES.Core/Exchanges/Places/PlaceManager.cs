using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Core.Commons;
using Microsoft.EntityFrameworkCore;

namespace BugChang.DES.Core.Exchanges.Places
{
    public class PlaceManager
    {
        private readonly IPlaceRepository _placeRepository;
        private readonly IPlaceWardenRepository _placeWardenRepository;

        public PlaceManager(IPlaceRepository placeRepository, IPlaceWardenRepository placeWardenRepository)
        {
            _placeRepository = placeRepository;
            _placeWardenRepository = placeWardenRepository;
        }

        public async Task<ResultEntity> AddOrUpdateAsync(Place place)
        {
            var resultEntity = new ResultEntity();
            var placeId = GetPlaceId(place.Name);

            if (place.Id == placeId || placeId == 0)
            {
                if (place.Id > 0)
                {
                    _placeRepository.Update(place);
                }
                else
                {
                    await _placeRepository.AddAsync(place);
                }
                resultEntity.Success = true;
            }
            else
            {
                resultEntity.Message = "名称不允许重复";
            }

            return resultEntity;
        }

        public async Task<ResultEntity> DeleteAsync(int placeId)
        {
            var resultEntity = new ResultEntity();
            var existChildren = _placeRepository.GetQueryable().Count(a => a.ParentId == placeId) > 0;
            if (existChildren)
            {
                resultEntity.Message = "请先删除子级交换场所！";
            }
            else
            {
                var place = await _placeRepository.GetByIdAsync(placeId);
                place.IsDeleted = true;
                resultEntity.Message = $"【{place.Name}】已删除";
                resultEntity.Success = true;
            }
            return resultEntity;
        }

        private int GetPlaceId(string name)
        {
            var id = _placeRepository.GetQueryable().Where(a => a.Name == name).Select(a => a.Id).FirstOrDefault();
            return id;
        }

        public async Task<IList<int>> GetPlaceWardenIds(int placeId)
        {
            var placeWardenIds = await _placeWardenRepository.GetQueryable().Where(a => a.PlaceId == placeId).Select(a => a.UserId).ToListAsync();
            return placeWardenIds;
        }

        public async Task<ResultEntity> AssignWarden(int placeId, List<int> wardenIds)
        {
            var result = new ResultEntity();
            var place = await _placeRepository.GetByIdAsync(placeId);
            var placeWardens = await _placeWardenRepository.GetQueryable().Include(a => a.Place).Include(a => a.User).ThenInclude(a=>a.Department).Where(a => a.PlaceId == placeId).ToListAsync();

            //记录原有数据
            result.Data = new
            {
                OldData = placeWardens.Select(a => a.User).Select(a => new { a.Id, a.UserName, a.DisplayName,a.Department.FullName }),
                NewData = wardenIds
            };

            if (placeWardens.Count > 0)
            {
                //删除原数据
                foreach (var boxObject in placeWardens)
                {
                    await _placeWardenRepository.DeleteByIdAsync(boxObject.Id);
                }
            }

            //添加新数据
            foreach (var useId in wardenIds)
            {
                var boxObject = new PlaceWarden
                {
                    PlaceId = placeId,
                    UserId = useId
                };
                await _placeWardenRepository.AddAsync(boxObject);
            }

            result.Success = true;
            result.Message = $"交换场所【{place.Name}】已成功分配管理员";
            return result;
        }
    }
}
