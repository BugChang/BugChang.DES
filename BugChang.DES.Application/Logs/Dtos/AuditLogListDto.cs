namespace BugChang.DES.Application.Logs.Dtos
{
    public class AuditLogListDto
    {
        public int Id { get; set; }

        public string Level { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public string OperatorName { get; set; }

        public string Data { get; set; }

        public string CreateTime { get; set; }
    }
}
