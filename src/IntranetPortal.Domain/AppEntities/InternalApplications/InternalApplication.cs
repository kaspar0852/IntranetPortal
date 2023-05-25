using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace IntranetPortal.InternalApplications
{
    public class InternalApplication : AuditedAggregateRoot<Guid>
    {
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public bool IsActive { get; set; }

        public string? Description { get; set; }

        public int OrderNumber { get; set; }

        public string? LogoUrl { get; set; }

        public string? ApplicationUrl { get; set; }
    }
}
