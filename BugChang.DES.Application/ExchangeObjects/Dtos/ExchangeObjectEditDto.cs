using BugChang.DES.Application.Commons;

namespace BugChang.DES.Application.ExchangeObjects.Dtos
{
    public class ExchangeObjectEditDto : EditDto
    {
        public string Name { get; set; }

        public int ObjectType { get; set; }

        public int Value { get; set; }

        public string ValueText { get; set; }

        public int? ParentId { get; set; }

        public bool IsVirtual { get; set; }

        /// <summary>
        /// 限制码
        /// </summary>
        public string RestrictionCode { get; set; }
    }
}
