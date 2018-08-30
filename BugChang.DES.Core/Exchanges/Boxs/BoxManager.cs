using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Core.Commons;
using Microsoft.EntityFrameworkCore;

namespace BugChang.DES.Core.Exchanges.Boxs
{
    public class BoxManager
    {
        private readonly IBoxRepository _boxRepository;
        private readonly IBoxObjectRepository _boxObjectRepository;

        public BoxManager(IBoxRepository boxRepository, IBoxObjectRepository boxObjectRepository)
        {
            _boxRepository = boxRepository;
            _boxObjectRepository = boxObjectRepository;
        }

        public async Task<ResultEntity> AddOrUpdateAsync(Box box)
        {
            var result = new ResultEntity();
            var exist = await _boxRepository.GetQueryable().Where(a => a.FrontBn == box.FrontBn && a.PlaceId == box.PlaceId && a.Id != box.Id).CountAsync() > 0;
            if (exist)
            {
                result.Message = "同一交换场所下BN号码不允许重复";
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

        public async Task<IList<int>> GetBoxObjectIds(int boxId)
        {
            var lstObjectId = await _boxObjectRepository.GetQueryable().Where(a => a.BoxId == boxId)
                .Select(a => a.ExchangeObjectId).ToListAsync();
            return lstObjectId;
        }

        public async Task<ResultEntity> AssignObject(int boxId, List<int> objectIds)
        {
            var result = new ResultEntity();
            var box = await _boxRepository.GetByIdAsync(boxId);
            var boxObjects = await _boxObjectRepository.GetQueryable().Include(a => a.Box).Include(a => a.ExchangeObject).Where(a => a.BoxId == boxId).ToListAsync();

            //记录原有数据
            result.Data = new
            {
                OldData = boxObjects.Select(a => a.ExchangeObject).Select(a => new { a.Id, a.Name, a.ValueText }),
                NewData = objectIds
            };

            if (boxObjects.Count > 0)
            {
                //删除原数据
                foreach (var boxObject in boxObjects)
                {
                    await _boxObjectRepository.DeleteByIdAsync(boxObject.Id);
                }
            }

            //添加新数据
            foreach (var objectId in objectIds)
            {
                var oldBox = await _boxObjectRepository.GetBoxByObjectId(objectId);
                if (oldBox!=null)
                {
                    if (oldBox.Id == boxId)
                    {
                        result.Message = "同一个流转对象只能分配一个箱格，请检查数据";
                        return result;
                    }
                }
               
                var boxObject = new BoxObject
                {
                    BoxId = boxId,
                    ExchangeObjectId = objectId
                };
                await _boxObjectRepository.AddAsync(boxObject);
            }

            result.Success = true;
            result.Message = $"箱格【{box.Name}】已成功分配流转对象";
            return result;
        }

        public async Task<ResultEntity> Cancel(int objectId)
        {
            var result = new ResultEntity();
            var box = await _boxObjectRepository.GetBoxByObjectId(objectId);
            if (box == null)
            {
                result.Message = "该流转对象不存在箱格，无法勘误";
            }
            else
            {
                box.FileCount -= 1;
                result.Success = true;
            }
            return result;
        }
    }
}
