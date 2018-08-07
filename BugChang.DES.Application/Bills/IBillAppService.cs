using System.Threading.Tasks;

namespace BugChang.DES.Application.Bills
{
    public interface IBillAppService
    {
        Task<int> CreateReceiveBill(int placeId, int objectId, int userId,int departmentId);
    }
}
