using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityNet.Event;
using System.Collections;

namespace UnityNet.Logging
{
    public class LoggerEvent :MyEvent
    {

        public static readonly string ONLOGGER = "onLogger";
    
        private LogLevel level;

        public LoggerEvent(LogLevel level, string message)
        {
            this.Type = ONLOGGER;
            this.level = level;
            ErrorDes = message;
        }

        public static string LogEventType(LogLevel level)
        {
            return ("LOG_" + level.ToString());
        }

        public object Clone()
        {
            return new LoggerEvent(this.level, ErrorDes);
        }

        public override string ToString()
        {
            return string.Format("LoggerEvent " + Type, new object[0]);
        }

        public LogLevel LogLevel
        {
            get { return this.level; }
        }

        public static bool ContainEventName(string eveName)
        {
            if (eveName.Equals(ONLOGGER))
            {
                return true;
            }
            return false;
        }
    }
}
