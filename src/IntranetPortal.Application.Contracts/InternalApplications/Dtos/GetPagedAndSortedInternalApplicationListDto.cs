using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace IntranetPortal.InternalApplicationDto
{
    public class GetPagedAndSortedInternalApplicationListDto : PagedAndSortedResultRequestDto
    {
        public string SearchKeyword { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set;}

        public string SortType { get; set; } = "asc";
    }
}
