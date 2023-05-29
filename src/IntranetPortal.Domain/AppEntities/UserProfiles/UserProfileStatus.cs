using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace IntranetPortal.AppEntities.UserProfiles
{
    public class UserProfileStatus : FullAuditedAggregateRoot<Guid>
    {
        public string SystemName { get; set; }
        public string DisplayName { get; set; }
    }
}
