using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace IntranetPortal.AppEntities.Documents
{
        public class Document : FullAuditedAggregateRoot<Guid>
        {
            public string Name { get; set; }
            public string? Description { get; set; }
            public string DocumentUrl { get; set; }
            public int AcknowledgementDeadlineInDays { get; set; }
            public Guid DocumentStatusId { get; set; }

        }
  
}
