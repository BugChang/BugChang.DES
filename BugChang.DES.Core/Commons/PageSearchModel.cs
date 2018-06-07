namespace BugChang.DES.Core.Commons
{
  public  class PageSearchModel
    {
        public int Skip { get; set; }

        public int Take { get; set; }

        public string Keywords { get; set; }

        public int? ParentId { get; set; }
    }
}
