using System;
using System.Collections;
using UnityNet.Core;

namespace UnityNet.Logging
{
    public class Logger
    {
        private bool enableConsoleTrace = false;
        private bool enableEventDispatching = true;
        private LogLevel loggingLevel;
        private AbstractEndpoint endpoint;

        public Logger(AbstractEndpoint endpoint)
        {
            this.endpoint = endpoint;
            this.loggingLevel = LogLevel.INFO;
        }

        public void Debug(params string[] messages)
        {
            this.Log(LogLevel.DEBUG, string.Join(" ", messages));
        }

        public void Error(params string[] messages)
        {
            this.Log(LogLevel.ERROR, string.Join(" ", messages));
        }

        public void Info(params string[] messages)
        {
            this.Log(LogLevel.INFO, string.Join(" ", messages));
        }

        private void Log(LogLevel level, string message)
        {
            if (level >= this.loggingLevel)
            {
                if (this.enableConsoleTrace)
                {
                    Console.WriteLine(string.Concat(new object[] { "[UnityNet - ", level, "] ", message }));
                }
                if (this.enableEventDispatching && (this.endpoint != null))
                {
                    LoggerEvent evt = new LoggerEvent(this.loggingLevel, message);
                    this.endpoint.Dispatch(evt);
                }
            }
        }

        public void Warn(params string[] messages)
        {
            this.Log(LogLevel.WARN, string.Join(" ", messages));
        }

        public bool EnableControlTrace
        {
            get
            {
                return this.enableConsoleTrace;
            }
            set
            {
                this.enableConsoleTrace = value;
            }
        }

        public bool EnableEventDispatching
        {
            get
            {
                return this.enableEventDispatching;
            }
            set
            {
                this.enableEventDispatching = value;
            }
        }

        public LogLevel LoggingLevel
        {
            get
            {
                return this.loggingLevel;
            }
            set
            {
                this.loggingLevel = value;
            }
        }
    }
}

