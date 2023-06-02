using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Documents.Dtos
{
    public class ReuploadDocumentInputDto
    {
        public Guid DocumentId { get; set; }

        public string DocumentUrl { get; set; }
    }
}
