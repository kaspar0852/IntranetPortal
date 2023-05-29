using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace IntranetPortal.AppEntities
{
    public class Designation : FullAuditedAggregateRoot<Guid>
    {
        public string Name { get; set; }
        public string ReferenceId { get; set; }
        public string? Code { get; set; }
        public bool IsActive { get; set; }
    }
}
