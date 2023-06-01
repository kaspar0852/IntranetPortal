using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace IntranetPortal.Documents.Dtos
{
    public class DocumentAcknowledgementRequestsForMeDto : AuditedEntityDto<Guid>
    {
        public Guid AcknowledgementRequestedToId { get; set; }
        public string AcknowledgementRequestedToFullName { get; set; }
        public Guid DocumentId { get; set; }
        public string DocumentName { get; set; }    
        public string DocumentDescription { get; set; }
        public string DocumentUrl { get; set; }
        public string CreatedByFullName { get; set; }
        public string LastModifiedByFullName { get; set; }
        public Guid DocumentAcknowledgementRequestStatusId { get; set; }
        public string DocumentAcknowledgementRequestStatusSystemName { get; set; }
        public string DocumentAcknowledgementRequestStatusDisplayName { get; set; }
        public DateTime? AcknowledgementDoneDateTime { get; set; }
        public DateTime DueDateTime { get; set; }
    }
}
