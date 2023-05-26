using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace IntranetPortal.Documents.Dtos
{
    public class DeactivateDocumentDto : AuditedEntityDto<Guid>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string DocumentUrl { get; set; }
        public Guid DocumentStatusId { get; set; }
        public string DocumentStatusSystemName { get; set; }
        public string DocumentStatusDisplayName { get; set; }
        public int AcknowledgementDeadlineInDays { get; set; }
        public string CreatedByFullName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedByFullname { get; set; }
        public DateTime LastModifiedDate { get; set; }

    }
}
