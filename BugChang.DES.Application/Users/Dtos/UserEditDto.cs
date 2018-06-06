namespace BugChang.DES.Application.Users.Dtos
{
    public class UserEditDto
    {
        public int Id { get; set; }

        public int DepartmentId { get; set; }

        public string UserName { get; set; }

        public string DisplayName { get; set; }

        public int Enabled { get; set; }

        public string Phone { get; set; }

        public string Tel { get; set; }
    }
}
