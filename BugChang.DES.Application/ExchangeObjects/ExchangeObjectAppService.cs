using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BugChang.DES.Application.ExchangeObjects.Dtos;
using BugChang.DES.Core.Commons;
using BugChang.DES.Core.Exchanges.ExchangeObjects;

namespace BugChang.DES.Application.ExchangeObjects
{
    public class ExchangeObjectAppService : IExchangeObjectAppService
    {
        public Task<ResultEntity> AddOrUpdateAsync(ExchangeObjectEditDto editDto)
        {
            throw new NotImplementedException();
        }

        public Task<ResultEntity> DeleteByIdAsync(int id, int userId)
        {
            throw new NotImplementedException();
        }

        public Task<ExchangeObjectEditDto> GetForEditByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<PageResultModel<ExchangeObjectListDto>> GetPagingAysnc(PageSearchModel pageSearchDto)
        {
            throw new NotImplementedException();
        }

        public IList<ObjectTypeListDto> GetObjectTypes()
        {
            var objectTypeList = new List<ObjectTypeListDto>();
            foreach (var item in Enum.GetValues(typeof(EnumObjectType)))
            {
                var objectType = new ObjectTypeListDto
                {
                    Id = (int)item,
                    Name = item.ToString()
                };
                objectTypeList.Add(objectType);
            }

            return objectTypeList;
        }
    }
}
