using System.Collections.Generic;
using System.Threading.Tasks;
using BugChang.DES.Application.Bills.Dtos;
using BugChang.DES.Core.Commons;

namespace BugChang.DES.Application.Bills
{
    public interface IBillAppService
    {
        Task<ResultEntity> CreateReceiveBill(int placeId, int objectId, int userId, int departmentId);

        Task<ResultEntity> CreateSendBill(int placeId, int userId, int departmentId);

        Task<ResultEntity> CreateReceiveSendBill(int placeId, int userId, int departmentId);

        Task<BillDto> GetBill(int id);

        Task<IList<BillDetailDto>> GetBillDetails(int billId);

        Task<PageResultModel<BillDto>> GetBills(PageSearchCommonModel pageSearch);
    }
}
