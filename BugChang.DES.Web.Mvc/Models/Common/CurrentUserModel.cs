namespace BugChang.DES.Web.Mvc.Models.Common
{
    public class CurrentUserModel
    {
        public int UserId { get; set; }

        public int DepartmentId { get; set; }

        public string UserName { get; set; }

        public string DisplayName { get; set; }

        public bool NeedChangePassword { get; set; }
    }
}
