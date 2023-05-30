using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace IntranetPortal.AppEntities.UserProfiles
{
    public class UserProfile : FullAuditedAggregateRoot<Guid>
    {
        public Guid AbpUserId { get; set; }
        public Guid? DesignationId { get; set; }
        public Guid? DepartmentId { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public DateTime HiredDate { get; set; }
        public string ReferenceId { get; set; }
        public string? MiddleName { get; set; }
        public Guid UserProfileStatusId { get; set; }
    }
}
