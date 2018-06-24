using BugChang.DES.Application.Commons;

namespace BugChang.DES.Application.Places.Dtos
{
    public class PlaceListDto : BaseDto
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        public string DepartmentName { get; set; }

        public string ParentName { get; set; }
    }
}
