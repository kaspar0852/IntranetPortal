using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Documents.Dtos
{
    public class RequestAcknowledgementForDocumentInputDto
    {
        public Guid DocumentId { get; set; }
        public List<Guid> AcknowledgmentRequestedToId { get; set; }
    }
}
