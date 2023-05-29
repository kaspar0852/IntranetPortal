using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Departments.Dtos
{
    public class GetDepartmentDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ReferenceId { get; set; }

        public string Code { get; set; }

        public bool IsActive { get; set; }
    }
}
