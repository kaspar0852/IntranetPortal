using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace IntranetPortal.AppEntities.Documents
{
    public class DocumentStatus : FullAuditedAggregateRoot<Guid>
    {
        public string SystemName { get; set; }
        public string DisplayName { get; set; }
    }
}
