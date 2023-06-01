using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace IntranetPortal.Documents.Dtos
{
    public class PagedAndSortedDocumentListDto : PagedAndSortedResultRequestDto
    {
        public string SearchKeyword { get; set; }
        public List<Guid> DocumentStatusId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public DateTime? AcknowledgedFromDate { get; set; }
        public DateTime? AcknowledgedToDate { get; set; }
        public string SortType { get; set; } = "asc";

    }
}
