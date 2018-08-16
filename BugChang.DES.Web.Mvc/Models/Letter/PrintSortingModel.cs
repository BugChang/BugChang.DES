using System.Collections.Generic;
using BugChang.DES.Application.Letters.Dtos;

namespace BugChang.DES.Web.Mvc.Models.Letter
{
    public class PrintSortingModel
    {
        public SortingListDto SortingList{ get; set; }

        public IList<LetterSortingDto> LetterSortings { get; set; }
    }
}
