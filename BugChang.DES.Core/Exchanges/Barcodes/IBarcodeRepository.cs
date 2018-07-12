using System.Threading.Tasks;
using BugChang.DES.Core.Commons;

namespace BugChang.DES.Core.Exchanges.Barcodes
{
    public interface IBarcodeRepository : IBasePageSearchRepository<Barcode>
    {
        Task<Barcode> GetByNoAsync(string barcode);
    }

    public interface IBarcodeLogRepository : IBasePageSearchRepository<Barcode>
    {

    }

    public interface IBarcodeRouteRepository : IBaseRepository<BarcodeRoute>
    {

    }
}
