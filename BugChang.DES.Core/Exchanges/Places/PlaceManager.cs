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

        private int GetPlaceId(string name)
        {
            var id = _placeRepository.GetQueryable().Where(a => a.Name == name).Select(a => a.Id).FirstOrDefault();
            return id;
        }
    }
}
