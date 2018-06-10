using System;

namespace BugChang.DES.Application.Commons
{
    public abstract class EditDto
    {
        public int Id { get; set; }
        public int? CreateBy { get; set; }

        public DateTime CreateTime { get; set; }

        public int? UpdateBy { get; set; }

        public DateTime? UpdateTime { get; set; }

        public void SetCreateOrUpdateInfo(int currentUserId)
        {
            if (Id > 0)
            {
                UpdateBy = currentUserId;
                UpdateTime = DateTime.Now;
            }
            else
            {
                CreateBy = currentUserId;
                CreateTime = DateTime.Now;
            }
        }

    }
}
