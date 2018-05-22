namespace BugChang.DES.Core.Organizations
{
    public class Organization : BaseEntity, ISoftDelete
    {

        public string Name { get; set; }

        public string FullName { get; set; }

        public string Code { get; set; }

        public int? ParentId { get; set; }


        public bool IsDeleted { get; set; }
    }
}
