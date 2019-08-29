using System;

namespace mscfreshman.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }
        public int StatusCode { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
