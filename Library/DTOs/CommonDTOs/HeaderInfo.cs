using System;
using System.Runtime.Serialization;

namespace CommonDTOs
{
    [DataContract(Name = "HeaderInfo")]
    public class HeaderInfo
    {
        [DataMember(Name = "AccessToken")]
        public string AccessToken { get; set; }

        [DataMember(Name = "TenantId")]
        public string TenantId { get; set; }

        [DataMember(Name = "UserId")]
        public string UserId { get; set; }

        [DataMember(Name = "TraceId")]
        public string TraceId { get; set; }

        [DataMember(Name = "Authorization")]
        public string Authorization { get; set; }

        [DataMember(Name = "ContextId")]
        public string ContextId { get; set; }

        [DataMember(Name = "AccessTokenType")]
        public AccessTokenType AccessTokenType { get; set; }

        [DataMember(Name = "UserAgent")]
        public string UserAgent { get; set; }

        [DataMember(Name = "TokenStatus")]
        public string TokenStatus { get; set; }

        [DataMember(Name = "RandomNumber")]
        public string RandomNumber { get; set; }
    }

    public enum AccessTokenType
    {
        Default,
        Share
    }

    public class TraceData
    {
        public HeaderInfo headerInfo { get; set; }
        public string traceId { get; set; }
        public string spanId { get; set; }
        public string operationName { get; set; }
    }
}
