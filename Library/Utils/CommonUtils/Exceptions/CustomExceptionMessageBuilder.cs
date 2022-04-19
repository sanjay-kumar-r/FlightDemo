using ServiceContracts.CustomException;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtils.Exceptions
{
    public class CustomExceptionMessageBuilder : ICustomExceptionMessageBuilder
    {
        private List<string> messages = new List<string>();
        public IEnumerable<string> Messages { get; set; }
        public void AddMessage(string message) => messages.Add(message);
    }
}
