using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Documents.Dtos
{
    public class CreateDocumentDto
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public string DocumentUrl { get; set; }

        public int AcknowledgementDeadlineInDays { get; set; }
    }
}
