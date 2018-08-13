using BugChang.DES.Application.Commons;

namespace BugChang.DES.Application.Users.Dtos
{
    public class UserListDto : BaseDto
    {
        public string UserName { get; set; }

        public string DisplayName { get; set; }

        public string DepartmentName { get; set; }

        public string UsbKeyNo { get; set; }

        public bool Enabled { get; set; }

        public bool Locked { get; set; }

        public string Phone { get; set; }

        public string Tel { get; set; }

    }
}
