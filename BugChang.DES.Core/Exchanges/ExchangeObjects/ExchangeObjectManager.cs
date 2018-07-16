using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Core.Commons;
using Microsoft.EntityFrameworkCore;

namespace BugChang.DES.Core.Exchanges.ExchangeObjects
{
    public class ExchangeObjectManager
    {
        private readonly IExchangeObjectRepository _exchangeObjectRepository;
        private readonly IExchangeObjectSignerRepository _objectSignerRepository;

        public ExchangeObjectManager(IExchangeObjectRepository exchangeObjectRepository, IExchangeObjectSignerRepository objectSignerRepository)
        {
            _exchangeObjectRepository = exchangeObjectRepository;
            _objectSignerRepository = objectSignerRepository;
        }

        public async Task<ResultEntity> AddOrUpdateAsync(ExchangeObject exchangeObject)
        {
            var resultEntity = new ResultEntity();
            var existQuery = _exchangeObjectRepository.GetQueryable().Where(a => a.Name == exchangeObject.Name);
            if (exchangeObject.Id > 0)
            {
                existQuery = existQuery.Where(a => a.Id != exchangeObject.Id);
            }

            if (await existQuery.CountAsync() > 0)
            {
                resultEntity.Message = "流转对象名称已存在";
            }
            else
            {
                if (exchangeObject.Id > 0)
                {
                    _exchangeObjectRepository.Update(exchangeObject);
                }
                else
                {
                    await _exchangeObjectRepository.AddAsync(exchangeObject);
                }

                resultEntity.Success = true;
            }

            return resultEntity;
        }

        public async Task<ResultEntity> DeleteByIdAsync(int id)
        {
            var resultEntity = new ResultEntity();
            var childrenCount = await _exchangeObjectRepository.GetQueryable().CountAsync(a => a.ParentId == id);
            if (childrenCount > 0)
            {
                resultEntity.Message = "请先删除该流转对象的所有子级";
            }
            else
            {
                var exchangeObject = await _exchangeObjectRepository.GetByIdAsync(id);
                exchangeObject.IsDeleted = true;
                resultEntity.Success = true;
            }
            return resultEntity;
        }

        public async Task<ResultEntity> AssignObjectSigner(int objectId, List<int> userIds)
        {
            var result = new ResultEntity();
            var exchangeObject = await _exchangeObjectRepository.GetByIdAsync(objectId);
            var objectSigners = await _objectSignerRepository.GetQueryable().Include(a => a.ExchangeObject).Include(a => a.User).ThenInclude(a => a.Department).Where(a => a.ExchangeObjectId == objectId).ToListAsync();

            //记录原有数据
            result.Data = new
            {
                OldData = objectSigners.Select(a => a.User).Select(a => new { a.Id, a.UserName, a.DisplayName, a.Department.FullName }),
                NewData = userIds
            };

            if (objectSigners.Count > 0)
            {
                //删除原数据
                foreach (var objectSigner in objectSigners)
                {
                    await _objectSignerRepository.DeleteByIdAsync(objectSigner.Id);
                }
            }

            //添加新数据
            foreach (var useId in userIds)
            {
                var boxObject = new ExchangeObjectSigner
                {
                    ExchangeObjectId = objectId,
                    UserId = useId
                };
                await _objectSignerRepository.AddAsync(boxObject);
            }

            result.Success = true;
            result.Message = $"流转对象【{exchangeObject.Name}】已成功分配签收人";
            return result;
        }

        public async Task<IList<int>> GetObjectSignerIds(int objectId)
        {
            var objectSignerIds =await _objectSignerRepository.GetQueryable().Where(a => a.ExchangeObjectId == objectId)
                .Select(a => a.UserId).ToListAsync();
            return objectSignerIds;
        }
    }
}
