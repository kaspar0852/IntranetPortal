using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.UserProfiles.Dtos
{
    public class UserProfileDto 
    {
        public Guid Id { get; set; }
        public Guid AbpUserId { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string MiddleName { get; set; }
        public string SurName { get; set; }
        public string Email { get; set; }   
        public Guid? DesignationId { get; set; }
        public string DesignationName { get; set; }
        public Guid? DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string ProfilePicutreUrl { get; set; }
        public DateTime HiredDate { get; set; }
        public DateTime DateOfBirth { get; set; }
        public  DateTime CreationTime { get; set; }
    }
}
