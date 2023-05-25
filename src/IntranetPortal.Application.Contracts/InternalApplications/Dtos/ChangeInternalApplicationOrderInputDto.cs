using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.InternalApplicationDto
{
    public class ChangeInternalApplicationOrderInputDto
    {
        public List<Guid> Id { get; set; }
        public List<int> OrderNumber { get; set; }
    }
}
