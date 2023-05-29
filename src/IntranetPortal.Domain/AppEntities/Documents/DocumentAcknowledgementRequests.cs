using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace IntranetPortal.AppEntities.Documents
{
    public class DocumentAcknowledgementRequests : FullAuditedAggregateRoot<Guid>
    {
        public Guid AbpUserId { get; set; }
        public Guid DocumentId { get; set; }
        public Guid DocumentAcknowledgementRequestStatusId { get; set; }
        public DateTime AcknowledgedDateTime { get; set; }
        public DateTime DueDateTime { get; set; }
        public virtual DocumentAcknowledgementRequestStatuses DocumentAcknowledgementRequestStatuses { get; set; }
    }
}
