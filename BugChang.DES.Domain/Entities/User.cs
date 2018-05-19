namespace BugChang.DES.Domain.Entities
{
    public class User : BaseEntity, IBasicEntity
    {
        public string UserName { get; set; }

        public string DisplayName { get; set; }

        public string Password { get; set; }

        public bool Enabled { get; set; }

        public int LoginErrorCount { get; set; }

        public string Phone { get; set; }

        public string Tel { get; set; }

    }
}
