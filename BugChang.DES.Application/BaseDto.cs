using System;

namespace BugChang.DES.Application
{
    public abstract class BaseDto
    {

        public int Id { get; set; }

        public string CreateUserName { get; set; }

        public string CreateTime { get; set; }

        public string UpdateUserName { get; set; }

        public string UpdateTime { get; set; }
    }
}
