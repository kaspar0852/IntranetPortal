using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Documents.Dtos
{
    public class GetDocumentAcknowledgementRequestStatusDto
    {
        public Guid Id { get; set; }
        public string SystemName { get; set; }
        public string DisplayName { get; set; }

    }
}
