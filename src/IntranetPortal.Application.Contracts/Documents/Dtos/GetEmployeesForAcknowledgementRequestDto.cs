using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace IntranetPortal.Documents.Dtos
{
    public class GetEmployeesForAcknowledgementRequestDto : PagedAndSortedResultRequestDto
    {
        public string? SearchKeyword { get; set; }
        public Guid DocumentId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string SortType { get; set; } = "asc";
        public Guid[]? DepartmentId { get; set; }
        public Guid[]? DesignationId { get; set; }

    }
}
