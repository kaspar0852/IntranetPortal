using System;

namespace IntranetPortal.Documents.Dtos
{
    public class ReponseForAcknowledgementRequestDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Middlename { get; set; }

        public string Surname { get; set; }

        public string FullName { get; set; }

        public string Designation { get; set; }

        public Guid DesignationId { get; set; }

        public string Department { get; set; }

        public Guid DepartmentId { get; set; }

        public string ProfilePictureUrl { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

    }
}
