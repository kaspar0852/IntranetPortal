using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace IntranetPortal.AppEntities.Documents
{
    public class DocumentAcknowledgementRequestStatuses : AggregateRoot<Guid>
    {
        public string SystemName { get; set; }

        public string DisplayName { get; set; }
    }
}
