using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceContracts.CustomException
{
    public interface ICustomExceptionMessageBuilder
    {
        public IEnumerable<string> Messages { get; }

        public void AddMessage(string message);
    }
}
