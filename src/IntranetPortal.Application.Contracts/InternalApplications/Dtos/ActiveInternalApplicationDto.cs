using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace IntranetPortal.InternalApplicationDto
{
    public class ActiveInternalApplicationDto : EntityDto<Guid>
    {
        public string Name { get; set; }
        public string Description { get; set; } 

        public string LogouRL { get; set; }

        public string ApplicationUrl { get; set; }

        public int OrderNumber { get; set; }
    }
}
