using System;

namespace CommonDTOs
{
    public class HeaderInfo
    {
        public string UserId { get; set; }
        public string TenantId { get; set; }
        public string AccessToken { get; set; }
    }

    public class TraceData
    {
        public HeaderInfo headerInfo { get; set; }
        public string traceId { get; set; }
        public string spanId { get; set; }
        public string operationName { get; set; }
    }
}
