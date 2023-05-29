using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Responses
{
    public class ResponseDto
    {
        public bool Success { get; set; }
        public int Code { get; set; }

        public string Message { get; set; }
    }
}
