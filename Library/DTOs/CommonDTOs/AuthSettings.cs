using System;
using System.Collections.Generic;
using System.Text;

namespace CommonDTOs
{
    public sealed class AuthSettings
    {
        public string Key { get; set; }
        public List<string> Roles { get; set; }
    }
}
