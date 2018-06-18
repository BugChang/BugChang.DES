using BugChang.DES.Core.Commons;

namespace BugChang.DES.Core.Authorization.Roles
{
  public  class RoleOperation:BaseEntity,ISoftDelete
    {
        public int RoleId { get; set; }

        public string OperationCode { get; set; }

        public bool IsDeleted { get; set; }
    }
}
