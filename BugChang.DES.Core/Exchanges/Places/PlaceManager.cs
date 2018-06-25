using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Core.Commons;

namespace BugChang.DES.Core.Exchanges.Places
{
    public class PlaceManager
    {
        private readonly IPlaceRepository _placeRepository;

        public PlaceManager(IPlaceRepository placeRepository)
        {
            _placeRepository = placeRepository;
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
    }
}
