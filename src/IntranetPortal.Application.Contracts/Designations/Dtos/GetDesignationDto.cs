using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Designations.Dtos
{
    public class GetDesignationDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ReferenceId { get; set; }

        public string Code { get; set; }

        public bool IsActive { get; set; }
    }
}
