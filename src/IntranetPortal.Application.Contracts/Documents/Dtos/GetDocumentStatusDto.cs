using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Documents.Dtos
{
    public class GetDocumentStatusDto
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; }

        public string SystemName { get; set; }
    }
}
