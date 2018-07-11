using System.Linq;
using System.Threading.Tasks;
using BugChang.DES.Core.Commons;
using Microsoft.EntityFrameworkCore;

namespace BugChang.DES.Core.Exchanges.ExchangeObjects
{
    public class ExchangeObjectManager
    {
        private readonly IExchangeObjectRepository _exchangeObjectRepository;

        public ExchangeObjectManager(IExchangeObjectRepository exchangeObjectRepository)
        {
            _exchangeObjectRepository = exchangeObjectRepository;
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
    }
}
