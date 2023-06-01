using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace IntranetPortal.Documents.Dtos
{
    public class PagedAndSortedDocumentAcknowledgementRequestListDto : PagedAndSortedResultRequestDto
    {
        public string SearchKeyword { get; set; }
        public string SortType { get; set; } = "asc";
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public DateTime? AcknowledgedFromDate { get; set; }
        public DateTime? AcknowledgedToDate { get; set; }
        public Guid DocumentId { get; set; }
        public List<Guid> DocumentAcknowledgementRequestStatusId { get; set; }

    }
}
