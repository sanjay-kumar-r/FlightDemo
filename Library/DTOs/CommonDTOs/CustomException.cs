using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace CommonDTOs
{
    [DataContract(Name = "CustomException")]
    public class CustomException : Exception
    {
        [DataMember(Name = "CustomErrorCode")]
        public string CustomErrorCode;

        [DataMember(Name = "CustomErrorMessage")]
        public string CustomErrorMessage;

        [DataMember(Name = "CustomStackTrace")]
        public string CustomStackTrace;
    }

    public static class CustomErrorCode
    {
        public const string Invalid = "E001";
        public const string Duplicate = "E002";
        public const string Unknown = "E003";

    }
}
