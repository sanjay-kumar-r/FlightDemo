using CommonDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceContracts.Logger
{
    public interface ILogger
    {
        void Log(LogLevel logLevel, Exception e, TraceData traceData = null);
        void Log(LogLevel logLevel, string user, Exception e, TraceData traceData = null);
        void Log(LogLevel logLevel, string message, TraceData traceData = null);

    }
}
