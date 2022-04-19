using CommonDTOs;
using log4net;
using log4net.Core;
using System;
using System.IO;
using System.Reflection;

namespace Utils.Logger
{
    public class LoggerUtils : ServiceContracts.Logger.ILogger
    {
        private static readonly ILog _logger;
        private static object syncHandle = new object();

        static LoggerUtils()
        {
            if (_logger == null)
            {
                lock (syncHandle)
                {
                    _logger = Initialize();
                }
            }
        }
        public LoggerUtils()
        {
        }

        private static ILog Initialize()
        {
            try
            {
                ILog logger;
                var logRepository = log4net.LogManager.CreateRepository(Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));
                FileInfo fileInfo = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "log4net.config");
                //read logpath from environment variable
                string currentdir = Directory.GetDirectoryRoot(System.Environment.CurrentDirectory);
                string envLogPath = System.Environment.GetEnvironmentVariable("WSLOGPATH");
                log4net.GlobalContext.Properties["WSLOGPATH"] = !string.IsNullOrWhiteSpace(envLogPath) ? envLogPath + Path.DirectorySeparatorChar
                    : currentdir + Path.DirectorySeparatorChar + "WSLOGFOLDER" + Path.DirectorySeparatorChar;
                log4net.Config.XmlConfigurator.Configure(logRepository, fileInfo);
                logger = LogManager.GetLogger(Assembly.GetEntryAssembly(), "Logger");
                dynamic currentLogger = logger.Logger;
                Level logLevel = null;
                //read loglevel from environment variable
                string envLogLevel = System.Environment.GetEnvironmentVariable("WSLOGLEVEL");
                if (!string.IsNullOrWhiteSpace(envLogLevel))
                {
                    //Try Getting the Level which matches envLogLevel
                    Object obj;
                    Enum.TryParse(typeof(LogLevel), envLogLevel, true, out obj);
                    if (obj != null && obj.GetType() == typeof(Level))
                    {
                        logLevel = (Level)obj;
                    }
                }
                //if environment variable is not set or invalid, then use same log4net.config Level or defaultTo-All
                if (logLevel == null && currentLogger.Level != null)
                    logLevel = currentLogger.Level;
                currentLogger.Level = (logLevel != null) ? logLevel : Level.All;
                return logger;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //public bool Reload()
        //{
        //    try
        //    {
        //        new Logger();
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //    return true;
        //}

        public void Shutdown()
        {
            try
            {
                log4net.LogManager.Shutdown();
            }
            catch
            {
            }
        }

        private void SetLogicalThreadContext(TraceData traceData)
        {
            SetLogicalThreadContext(traceData?.spanId, traceData?.traceId, traceData?.operationName,
                traceData.headerInfo?.UserId, traceData?.headerInfo?.TenantId);
        }

        public static void SetLogicalThreadContext(string spanId, string traceId, string operationName, string userId, string tenantId)
        {
            log4net.LogicalThreadContext.Properties["SpanId"] = spanId;
            log4net.LogicalThreadContext.Properties["TraceId"] = traceId;
            log4net.LogicalThreadContext.Properties["OperationName"] = operationName;
            log4net.LogicalThreadContext.Properties["UserId"] = userId;
            log4net.LogicalThreadContext.Properties["TenantId"] = tenantId;
        }

        public void Log(LogLevel logLevel, Exception e, TraceData traceData = null)
        {
            Log(logLevel, null, e, traceData);
        }

        public void Log(LogLevel logLevel, string user, Exception e, TraceData traceData = null)
        {
            if (traceData != null)
                SetLogicalThreadContext(traceData);
            string message = (user != null) ? "Application user: " + user + "" : string.Empty;

            switch (logLevel)
            {
                case LogLevel.DEBUG:
                    if (_logger.IsDebugEnabled)
                        _logger.Debug(message, e);
                    break;

                case LogLevel.INFO:
                    if (_logger.IsInfoEnabled)
                        _logger.Info(message, e);
                    break;

                case LogLevel.WARN:
                    if (_logger.IsWarnEnabled)
                        _logger.Warn(message, e);
                    break;

                case LogLevel.FATAL:
                    if (_logger.IsFatalEnabled)
                        _logger.Fatal(message, e);
                    break;

                case LogLevel.ERROR:
                    if (_logger.IsErrorEnabled)
                        _logger.Error(message, e);
                    break;

                default:
                    break;
            }
        }

        public void Log(LogLevel logLevel, string message, TraceData traceData = null)
        {
            if (traceData != null)
                SetLogicalThreadContext(traceData);
            switch (logLevel)
            {
                case LogLevel.DEBUG:
                    if (_logger.IsDebugEnabled)
                        _logger.Debug(message);
                    break;

                case LogLevel.INFO:
                    if (_logger.IsInfoEnabled)
                        _logger.Info(message);
                    break;

                case LogLevel.ERROR:
                    if (_logger.IsErrorEnabled)
                        _logger.Error(message);
                    break;

                case LogLevel.FATAL:
                    if (_logger.IsFatalEnabled)
                        _logger.Fatal(message);
                    break;

                case LogLevel.WARN:
                    if (_logger.IsWarnEnabled)
                        _logger.Warn(message);
                    break;

                default:
                    break;
            }
        }
    }

    public static class ExceptionExtensions
    {
        public static string ToClearFormat(this Exception e, string method)
        {
            return string.Format("Server Error - [{0}]: {1} ::: Message={2}{3}StackTrace={4}{5}Source={6}{7}",
                    DateTime.Now.Ticks,
                    method,
                    e.Message,
                    Environment.NewLine,
                    e.StackTrace,
                    Environment.NewLine,
                    e.Source,
                    Environment.NewLine);
        }
    }
}
