using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.InternalApplicationDto
{
    public class GetInternalApplicationDto
    {
        public string Description { get; set; }
        public string Name { get; set; }
        public string? DisplayName { get; set; }
        public bool IsActive { get; set; }
        public int OrderNumber { get; set; }
        public string? LogoUrl { get; set; }
        public string? ApplicationUrl { get; set; }
    }
}
