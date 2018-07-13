using System.Threading.Tasks;
using BugChang.DES.Core.Commons;

namespace BugChang.DES.Core.Exchanges.Barcodes
{
    public class BarcodeManager
    {
        /// <summary>
        /// 分配条码路由
        /// </summary>
        /// <param name="barcodeNo">条码号</param>
        /// <returns></returns>
        public Task<ResultEntity> AssignBarcodeRoute(string barcodeNo)
        {
            return new Task<ResultEntity>(() => new ResultEntity());
        }
    }
}
