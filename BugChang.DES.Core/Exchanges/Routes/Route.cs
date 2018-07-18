using BugChang.DES.Core.Commons;

namespace BugChang.DES.Core.Exchanges.Routes
{
    public class Route : BaseEntity<int>, ISoftDelete
    {
        public string BarcodeNo { get; set; }

        public int ObjectId { get; set; }

        public int Order { get; set; }

        public bool Completed { get; set; }

        public EnumRouteType RouteType { get; set; }

        public bool IsDeleted { get; set; }
    }
}
